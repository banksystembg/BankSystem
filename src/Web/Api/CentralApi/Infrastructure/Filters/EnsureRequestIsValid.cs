namespace CentralApi.Infrastructure.Filters
{
    using System;
    using System.Linq;
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
                var encryptedKey = authHeader[0].Remove(0, GlobalConstants.AuthenticationScheme.Length).Trim();
                var encryptedIv = authHeader[1];
                var incomingData = authHeader[2];

                var isValid = this.IsValidRequest(context, encryptedKey, encryptedIv, incomingData)
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
            string encryptedKey,
            string encryptedIv,
            string incomingData)
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

                // Decrypt
                string decrypted;
                using (var rsa = RSA.Create())
                {
                    RsaExtensions.FromXmlString(rsa, this.configuration?.Key);
                    var decryptedKey = rsa.Decrypt(Convert.FromBase64String(encryptedKey), RSAEncryptionPadding.Pkcs1);
                    var decryptedIv = rsa.Decrypt(Convert.FromBase64String(encryptedIv), RSAEncryptionPadding.Pkcs1);

                    decrypted = CryptographyExtensions.Decrypt(Convert.FromBase64String(incomingData), decryptedKey, decryptedIv);
                }

                var dataArgs = decrypted
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .ToArray();

                var bank = await bankService.GetBankAsync<BankServiceModel>(dataArgs[0], dataArgs[1], dataArgs[2]);
                if (bank.ApiKey == null)
                {
                    return false;
                }

                var serializedModel = JsonConvert.SerializeObject(model);
                var signature = Encoding.UTF8.GetBytes(serializedModel);

                // Verify signature with bank api key
                using (var rsa = RSA.Create())
                {
                    RsaExtensions.FromXmlString(rsa, bank.ApiKey);
                    var decrypt = Convert.FromBase64String(dataArgs[3]);
                    var isVerified = rsa.VerifyData(signature, decrypt, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

                    return isVerified;
                }
            }

            return false;
        }
    }
}