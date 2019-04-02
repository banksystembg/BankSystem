namespace CentralApi.Infrastructure.Filters
{
    using System;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading.Tasks;
    using BankSystem.Common;
    using BankSystem.Common.Utils;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;
    using Services.Implementations;
    using Services.Interfaces;
    using Services.Models.Banks;

    public class EnsureRequestIsValid : ActionFilterAttribute
    {
        private CentralApiConfiguration configuration;

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
                var bankName = authHeader[0].Remove(0, GlobalConstants.AuthenticationScheme.Length).Trim();
                var bankSwiftCode = authHeader[1];
                var bankCountry = authHeader[2];
                var encryptedKey = authHeader[3];
                var encryptedIV = authHeader[4];
                var incomingBase64Signature = authHeader[5];

                var isValid = this.IsValidRequest(context, bankName, bankSwiftCode, bankCountry, encryptedKey, encryptedIV, incomingBase64Signature)
                    .GetAwaiter()
                    .GetResult();

                if (!isValid)
                {
                    context.Result = new ForbidResult();

                    return;
                }
            }

            base.OnActionExecuting(context);
        }

        private async Task<bool> IsValidRequest(
            ActionExecutingContext context,
            string bankName,
            string bankSwiftCode,
            string bankCountry,
            string encryptedKey,
            string encryptedIV,
            string incomingBase64Signature)
        {
            var request = context.HttpContext.Request;
            var bankService = request.HttpContext.RequestServices.GetService(typeof(IBanksService)) as BanksService;
            var configOptions = request.HttpContext.RequestServices
                .GetService(typeof(IOptions<CentralApiConfiguration>)) as IOptions<CentralApiConfiguration>;
            this.configuration = configOptions?.Value;

            var actionArguments = context.ActionArguments;
            if (actionArguments.Values != null)
            {
                var model = ActionArgumentsUtil.GetModel(actionArguments);

                if (model == null)
                {
                    return false;
                }

                var bank = await bankService.GetBankAsync<BankServiceModel>(bankName, bankSwiftCode, bankCountry);
                if (bank.ApiKey == null)
                {
                    return false;
                }

                var serializedModel = JsonConvert.SerializeObject(model);
                var signature = Encoding.UTF8.GetBytes(serializedModel);

                // Decrypt
                string decrypted;
                using (var rsa = RSA.Create())
                {
                    RsaExtensions.FromXmlString(rsa, this.configuration?.Key);
                    var decryptedKey = rsa.Decrypt(Convert.FromBase64String(encryptedKey), RSAEncryptionPadding.Pkcs1);
                    var decryptedIV = rsa.Decrypt(Convert.FromBase64String(encryptedIV), RSAEncryptionPadding.Pkcs1);

                    decrypted = CryptographyExtensions.Decrypt(Convert.FromBase64String(incomingBase64Signature), decryptedKey, decryptedIV);
                }

                // Verify signature with bank api key
                using (var rsa = RSA.Create())
                {
                    RsaExtensions.FromXmlString(rsa, bank.ApiKey);
                    var decrypt = Convert.FromBase64String(decrypted);
                    var isVerified = rsa.VerifyData(signature, decrypt, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

                    return isVerified;
                }
            }

            return false;
        }
    }
}