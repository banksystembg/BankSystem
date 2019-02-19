namespace BankSystem.Web.Infrastructure.Handlers
{
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web;

    public class CustomDelegatingHandler : DelegatingHandler
    {
        private readonly string appId;
        private readonly string apiKey;

        public CustomDelegatingHandler(string appId, string apiKey)
        {
            this.appId = appId;
            this.apiKey = apiKey;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var requestUri = HttpUtility.UrlEncode(request.RequestUri.AbsoluteUri.ToLower());
            var requestHttpMethod = request.Method;

            //Raw signature string
            var signatureRawData =
                $"{this.appId}{requestHttpMethod}{requestUri}";

            var secretKeyByteArray = Convert.FromBase64String(this.apiKey);
            var signature = Encoding.UTF8.GetBytes(signatureRawData);

            using (var hmac = new HMACSHA256(secretKeyByteArray))
            {
                var signatureBytes = hmac.ComputeHash(signature);
                var requestSignatureBase64String = Convert.ToBase64String(signatureBytes);

                //Setting the values in the Authorization header using custom scheme (bsw)
                request.Headers.Authorization = new AuthenticationHeaderValue("bsw",
                    $"{this.appId},{requestSignatureBase64String}");
            }

            var response = await base.SendAsync(request, cancellationToken);
            return response;
        }
    }
}
