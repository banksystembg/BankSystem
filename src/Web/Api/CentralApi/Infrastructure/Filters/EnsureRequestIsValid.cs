namespace CentralApi.Infrastructure.Filters
{
    using System;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading.Tasks;
    using BankSystem.Common.Utils;
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
            var isValid = this.IsValidRequest(context)
                .GetAwaiter()
                .GetResult();

            if (!isValid)
            {
                context.Result = new ForbidResult();

                return;
            }

            base.OnActionExecuting(context);
        }

        private async Task<bool> IsValidRequest(
            ActionExecutingContext context)
        {
            var request = context.HttpContext.Request;
            var bankService = request.HttpContext.RequestServices.GetService(typeof(IBanksService)) as BanksService;
            var configOptions = request.HttpContext.RequestServices
                .GetService(typeof(IOptions<CentralApiConfiguration>)) as IOptions<CentralApiConfiguration>;
            this.configuration = configOptions?.Value;

            var actionArguments = context.ActionArguments;
            var model = actionArguments.Values?.First();

            if (model == null)
            {
                return false;
            }

            var incomingData = Encoding.UTF8.GetString(Convert.FromBase64String(model.ToString()));
            dynamic deserializedData = JsonConvert.DeserializeObject(incomingData);
            var bankName = deserializedData.BankName.ToString();
            var bankSwiftCode = deserializedData.BankSwiftCode.ToString();
            var bankCountry = deserializedData.BankCountry.ToString();

            var bank = await bankService.GetBankAsync<BankServiceModel>(bankName, bankSwiftCode,
                bankCountry);

            if (bank == null)
            {
                return false;
            }

            var encryptedKey = deserializedData.EncryptedKey.ToString();
            var encryptedIv = deserializedData.EncryptedIv.ToString();
            var data = deserializedData.Data.ToString();
            var signature = deserializedData.SignedData.ToString();

            // Decrypt
            string decrypted;
            using (var rsa = RSA.Create())
            {
                RsaExtensions.FromXmlString(rsa, this.configuration?.Key);
                var decryptedKey = rsa.Decrypt(Convert.FromBase64String(encryptedKey), RSAEncryptionPadding.Pkcs1);
                var decryptedIv = rsa.Decrypt(Convert.FromBase64String(encryptedIv), RSAEncryptionPadding.Pkcs1);

                decrypted = CryptographyExtensions.Decrypt(Convert.FromBase64String(data), decryptedKey, decryptedIv);
            }

            // Verify
            using (var rsa = RSA.Create())
            {
                RsaExtensions.FromXmlString(rsa, bank.ApiKey);

                var decrypt = Encoding.UTF8.GetBytes(decrypted);
                var isVerified = rsa.VerifyData(decrypt, Convert.FromBase64String(signature), HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

                return isVerified;
            }
        }
    }
}