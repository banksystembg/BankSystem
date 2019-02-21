namespace CentralApi.Infrastructure.Handlers
{
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Security.Cryptography;
    using System.Threading;
    using System.Threading.Tasks;
    using BankSystem.Common.Utils;

    public class CustomCentralApiDelegatingHandler : DelegatingHandler
    {
        private readonly string apiSigningKey;
        private readonly string bankKey;

        public CustomCentralApiDelegatingHandler(string apiSigningKey, string bankKey)
        {
            this.apiSigningKey = apiSigningKey;
            this.bankKey = bankKey;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Sign data with api private key
            string requestSignedData;
            using (var rsa = RSA.Create())
            {
                RsaExtensions.FromXmlString(rsa, this.apiSigningKey);

                var content = await request.Content.ReadAsByteArrayAsync();
                var signedData = rsa.SignData(content, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

                // Encrypt data with bank public key
                // We might use AES because the data might exceed the limit (490-500 bytes) and it will not be able to encrypt correctly
                using (var encryptionRsa = RSA.Create())
                {
                    RsaExtensions.FromXmlString(encryptionRsa, this.bankKey);
                    requestSignedData = Convert.ToBase64String(encryptionRsa.Encrypt(signedData, RSAEncryptionPadding.Pkcs1));
                }
            }

            //Setting the values in the Authorization header using custom scheme (bsw)
            request.Headers.Authorization = new AuthenticationHeaderValue("bsw",
                $"{requestSignedData}");

            var response = await base.SendAsync(request, cancellationToken);
            return response;
        }
    }
}
