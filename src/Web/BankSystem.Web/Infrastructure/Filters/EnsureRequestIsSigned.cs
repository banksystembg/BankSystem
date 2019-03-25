namespace BankSystem.Web.Infrastructure.Filters
{
    using Common;
    using Common.Configuration;
    using Common.Utils;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;
    using System;
    using System.Security.Cryptography;
    using System.Text;

    public class EnsureRequestIsSigned : ActionFilterAttribute
    {
        private BankConfiguration configuration;

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var request = context.HttpContext.Request;
            // check the scheme and auth header
            if (!request.Headers.ContainsKey(GlobalConstants.AuthorizationHeader) && !request.Headers[GlobalConstants.AuthorizationHeader][0]
                    .Contains(GlobalConstants.AuthenticationScheme, StringComparison.Ordinal))
            {
                context.Result = new ForbidResult();
                return;
            }

            var authHeader = request.Headers.GetCommaSeparatedValues(GlobalConstants.AuthorizationHeader);
            if (authHeader != null)
            {
                var encryptedKey = authHeader[0].Remove(0, GlobalConstants.AuthenticationScheme.Length).Trim();
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

            var actionArguments = context.ActionArguments;
            if (actionArguments.Values != null)
            {
                var model = ActionArgumentsUtil.GetModel(actionArguments);

                if (model == null)
                {
                    return false;
                }

                var serializedModel = JsonConvert.SerializeObject(model);
                var signature = Encoding.UTF8.GetBytes(serializedModel);

                var configOptions = request.HttpContext.RequestServices
                    .GetService(typeof(IOptions<BankConfiguration>)) as IOptions<BankConfiguration>;
                this.configuration = configOptions?.Value;

                // Decrypt
                string decrypted;
                using (var rsa = RSA.Create())
                {
                    RsaExtensions.FromXmlString(rsa, this.configuration?.Key);
                    var decryptedKey = rsa.Decrypt(Convert.FromBase64String(encryptedKey), RSAEncryptionPadding.Pkcs1);
                    var decryptedIV = rsa.Decrypt(Convert.FromBase64String(encryptedIV), RSAEncryptionPadding.Pkcs1);

                    decrypted = CryptographyExtensions.Decrypt(Convert.FromBase64String(incomingBase64Signature), decryptedKey, decryptedIV);
                }

                // Verify signature with central api key
                using (var rsa = RSA.Create())
                {
                    RsaExtensions.FromXmlString(rsa, this.configuration?.CentralApiPublicKey);
                    var isVerified = rsa
                        .VerifyData(signature, Convert.FromBase64String(decrypted), HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                    return isVerified;
                }
            }

            return false;
        }
    }
}
