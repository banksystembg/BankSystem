namespace CentralApi.Infrastructure.Filters
{
    using System;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading.Tasks;
    using BankSystem.Common;
    using Data;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.EntityFrameworkCore;
    using Models;
    using Newtonsoft.Json;

    public class EnsureRequestIsValid : ActionFilterAttribute
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
                var bankName = authHeader[0].Remove(0, AuthenticationScheme.Length).Trim();
                var bankSwiftCode = authHeader[1];
                var incomingBase64Signature = authHeader[2];

                var isValid = this.IsValidRequest(context, bankName, bankSwiftCode, incomingBase64Signature).GetAwaiter().GetResult();

                if (!isValid)
                {
                    context.Result = new ForbidResult();
                    return;
                }
            }

            base.OnActionExecuting(context);
        }

        private async Task<bool> IsValidRequest(ActionExecutingContext context, string bankName, string bankSwiftCode, string incomingBase64Signature)
        {
            var request = context.HttpContext.Request;
            var dbContext = request.HttpContext.RequestServices.GetService(typeof(CentralApiDbContext)) as CentralApiDbContext;

            var actionArguments = context.ActionArguments;
            if (actionArguments.ContainsKey(Id))
            {
                var model = actionArguments[Id] as ReceiveTransactionModel;

                var bankApiKey = await dbContext.Banks
                    .Where(b => string.Equals(b.Name, bankName, StringComparison.CurrentCultureIgnoreCase) &&
                                string.Equals(b.SwiftCode, bankSwiftCode, StringComparison.CurrentCultureIgnoreCase))
                    .Select(b => b.ApiKey)
                    .SingleOrDefaultAsync();

                if (bankApiKey == null)
                {
                    return false;
                }

                var test = JsonConvert.SerializeObject(model);
                var content = Encoding.UTF8.GetBytes(test);
                var requestContentBase64String = Convert.ToBase64String(content);

                var signature = Encoding.UTF8.GetBytes(requestContentBase64String);
                var requestSignatureBase64String = Convert.FromBase64String(incomingBase64Signature);

                var rsa = RSA.Create();
                RsaExtensions.FromXmlString(rsa, bankApiKey);

                var isVerified = rsa.VerifyData(signature, requestSignatureBase64String, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

                return isVerified;
            }

            return false;
        }
    }
}
