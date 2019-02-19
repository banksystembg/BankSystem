namespace CentralApi.Infrastructure.Filters
{
    using System;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web;
    using Data;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Http.Extensions;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.EntityFrameworkCore;

    public class EnsureRequestIsValid : ActionFilterAttribute
    {
        private const string AuthenticationScheme = "bsw";

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var request = context.HttpContext.Request;
            // check the scheme and auth header
            if (!request.Headers.ContainsKey("Authorization") && !request.Headers["Authorization"][0]
                    .Contains(AuthenticationScheme, StringComparison.Ordinal))
            {
                context.Result = new ForbidResult();
            }

            var authHeader = request.Headers.GetCommaSeparatedValues("Authorization");
            if (authHeader != null)
            {
                var appId = authHeader[0].Remove(0, AuthenticationScheme.Length).Trim();
                var incomingBase64Signature = authHeader[1];

                var isValid = this.IsValidRequest(context, appId, incomingBase64Signature).GetAwaiter().GetResult();

                if (!isValid)
                {
                    context.Result = new ForbidResult();
                }
            }

            base.OnActionExecuting(context);
        }

        private async Task<bool> IsValidRequest(ActionContext context, string appId, string incomingBase64Signature)
        {
            var request = context.HttpContext.Request;
            var requestUri = HttpUtility.UrlEncode(request.GetEncodedUrl().ToLower());
            var requestHttpMethod = request.Method;
            var dbContext = request.HttpContext.RequestServices.GetService(typeof(CentralApiDbContext)) as CentralApiDbContext;

            var bankApiKey = await dbContext.Banks
                .Where(b => b.AppId == appId)
                .Select(b => b.ApiKey)
                .SingleOrDefaultAsync();

            if (bankApiKey == null)
            {
                return false;
            }

            // TODO: Add security for MITM attacks

            var data = $"{appId}{requestHttpMethod}{requestUri}";

            var secretKeyBytes = Convert.FromBase64String(bankApiKey);
            var signature = Encoding.UTF8.GetBytes(data);

            using (var hmac = new HMACSHA256(secretKeyBytes))
            {
                var signatureBytes = hmac.ComputeHash(signature);
                var requestSignatureBase64String = Convert.ToBase64String(signatureBytes);

                return incomingBase64Signature.Equals(requestSignatureBase64String, StringComparison.Ordinal);
            }

        }
    }
}
