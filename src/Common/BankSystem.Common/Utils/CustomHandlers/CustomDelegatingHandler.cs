namespace BankSystem.Common.Utils.CustomHandlers
{
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Security.Cryptography;
    using System.Threading;
    using System.Threading.Tasks;

    public class CustomDelegatingHandler : DelegatingHandler
    {
        private readonly string apiSigningKey;
        private readonly string bankKey;
        private readonly string bankName;
        private readonly string bankSwiftCode;

        public CustomDelegatingHandler(string apiSigningKey, string bankKey)
        {
            this.apiSigningKey = apiSigningKey;
            this.bankKey = bankKey;
        }

        public CustomDelegatingHandler(string apiSigningKey, string bankKey, string bankName, string bankSwiftCode)
        {
            this.apiSigningKey = apiSigningKey;
            this.bankKey = bankKey;
            this.bankName = bankName;
            this.bankSwiftCode = bankSwiftCode;
        }


        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Sign data with bank private key
            string requestSignedData;
            using (var rsa = RSA.Create())
            {
                RsaExtensions.FromXmlString(rsa, this.bankKey);

                var content = await request.Content.ReadAsByteArrayAsync();
                var signedData = rsa.SignData(content, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

                // Encrypt data with central api public key
                // We might use AES because the data might exceed the limit (490-500 bytes) and it will not be able to encrypt correctly
                using (var encryptionRsa = RSA.Create())
                {
                    RsaExtensions.FromXmlString(encryptionRsa, this.apiSigningKey);
                    requestSignedData = Convert.ToBase64String(encryptionRsa.Encrypt(signedData, RSAEncryptionPadding.Pkcs1));
                }
            }

            //Setting the values in the Authorization header using custom scheme (bsw)
            request.Headers.Authorization = new AuthenticationHeaderValue("bsw",
                $"{this.bankName},{this.bankSwiftCode},{requestSignedData}");

            var response = await base.SendAsync(request, cancellationToken);
            return response;
        }
    }
}
