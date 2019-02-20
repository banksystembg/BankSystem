namespace BankSystem.Common.Utils.CustomHandlers
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
        private readonly string key;
        private readonly string bankName;
        private readonly string bankSwiftCode;

        public CustomDelegatingHandler(string key, string bankName, string bankSwiftCode)
        {
            this.key = key;
            this.bankName = bankName;
            this.bankSwiftCode = bankSwiftCode;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var rsa = RSA.Create();
            RsaExtensions.FromXmlString(rsa, this.key);

            // TODO: Encrypt data

            var content = await request.Content.ReadAsByteArrayAsync();
            var requestContentBase64String = Convert.ToBase64String(content);

            var signature = Encoding.UTF8.GetBytes(requestContentBase64String);
            var signedData = rsa.SignData(signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            var requestSignedData = Convert.ToBase64String(signedData);
            //Setting the values in the Authorization header using custom scheme (bsw)
            request.Headers.Authorization = new AuthenticationHeaderValue("bsw",
                $"{this.bankName},{this.bankSwiftCode},{requestSignedData}");

            var response = await base.SendAsync(request, cancellationToken);
            return response;
        }
    }
}
