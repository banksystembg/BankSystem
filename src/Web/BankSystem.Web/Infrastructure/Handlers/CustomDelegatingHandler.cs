namespace BankSystem.Web.Infrastructure.Handlers
{
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

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
            var requestContentBase64String = string.Empty;
            var requestUri = System.Web.HttpUtility.UrlEncode(request.RequestUri.AbsoluteUri.ToLower());
            var requestHttpMethod = request.Method.Method;

            //Calculate UNIX time
            var epochStart = new DateTime(1970, 01, 01, 0, 0, 0, 0, DateTimeKind.Utc);
            var timeSpan = DateTime.UtcNow - epochStart;
            var requestTimeStamp = Convert.ToUInt64(timeSpan.TotalSeconds).ToString();

            //create random nonce for each request
            var nonce = Guid.NewGuid().ToString("N");

            if (request.Content != null)
            {
                var content = await request.Content.ReadAsByteArrayAsync();
                var sha256 = SHA256.Create();

                //Hashing the request body
                var requestContentHash = sha256.ComputeHash(content);
                requestContentBase64String = Convert.ToBase64String(requestContentHash);
            }

            //Raw signature string
            var signatureRawData =
                $"{this.appId}{requestHttpMethod}{requestUri}{requestTimeStamp}{nonce}{requestContentBase64String}";

            var secretKeyByteArray = Convert.FromBase64String(this.apiKey);

            var signature = Encoding.UTF8.GetBytes(signatureRawData);

            using (var hmac = new HMACSHA256(secretKeyByteArray))
            {
                var signatureBytes = hmac.ComputeHash(signature);
                var requestSignatureBase64String = Convert.ToBase64String(signatureBytes);

                //Setting the values in the Authorization header using custom scheme (bsw)
                request.Headers.Authorization = new AuthenticationHeaderValue("bsw",
                    $"{this.appId}:{requestSignatureBase64String}:{nonce}:{requestTimeStamp}");
            }

            var response = await base.SendAsync(request, cancellationToken);
            return response;
        }
    }
}
