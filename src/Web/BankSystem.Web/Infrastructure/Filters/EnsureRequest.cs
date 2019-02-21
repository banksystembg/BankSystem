namespace BankSystem.Web.Infrastructure.Filters
{
    using System;
    using System.Security.Cryptography;
    using System.Text;
    using Api.Models;
    using Common.Utils;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.Extensions.Configuration;
    using Newtonsoft.Json;

    public class EnsureRequest : ActionFilterAttribute
    {
        private const string Id = "model";
        private const string AuthenticationScheme = "bsw";

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var request = context.HttpContext.Request;
            // check the scheme and auth header
            if (!request.Headers.ContainsKey("Authorization") && !request.Headers["Authorization"][0]
                    .Contains(AuthenticationScheme, StringComparison.Ordinal))
            {
                context.Result = new ForbidResult();
                return;
            }

            var authHeader = request.Headers.GetCommaSeparatedValues("Authorization");
            if (authHeader != null)
            {
                var encryptedKey = authHeader[0].Remove(0, AuthenticationScheme.Length).Trim();
                var encryptedIV = authHeader[1];
                var incomingBase64Signature = authHeader[2];
                var isValid = this.IsValidRequest(context, encryptedKey, encryptedIV, incomingBase64Signature);

                if (!isValid)
                {
                    context.Result = new ForbidResult();
                    return;
                }
            }

            base.OnActionExecuting(context);
        }

        private bool IsValidRequest(ActionExecutingContext context, string encryptedKey, string encryptedIV, string incomingBase64Signature)
        {
            var request = context.HttpContext.Request;
            var configuration = request.HttpContext.RequestServices.GetService(typeof(IConfiguration)) as IConfiguration;

            var actionArguments = context.ActionArguments;
            if (actionArguments.ContainsKey(Id))
            {
                var model = actionArguments[Id] as ReceiveMoneyTransferModel;

                var serializedModel = JsonConvert.SerializeObject(model);
                var signature = Encoding.UTF8.GetBytes(serializedModel);

                // Decrypt
                string decrypted;
                using (var rsa = RSA.Create())
                {
                    RsaExtensions.FromXmlString(rsa, configuration.GetSection("BankConfiguration:Key").Value);
                    var decryptedKey = rsa.Decrypt(Convert.FromBase64String(encryptedKey), RSAEncryptionPadding.Pkcs1);
                    var decryptedIV = rsa.Decrypt(Convert.FromBase64String(encryptedIV), RSAEncryptionPadding.Pkcs1);

                    decrypted = CryptographyExtensions.Decrypt(Convert.FromBase64String(incomingBase64Signature), decryptedKey, decryptedIV);
                }

                // Verify signature with central api key
                using (var rsa = RSA.Create())
                {
                    RsaExtensions.FromXmlString(rsa, configuration.GetSection("BankConfiguration:CentralApiPublicKey").Value);
                    var isVerified = rsa.VerifyData(signature, Convert.FromBase64String(decrypted), HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                    return isVerified;
                }
            }

            return false;
        }
    }
}
