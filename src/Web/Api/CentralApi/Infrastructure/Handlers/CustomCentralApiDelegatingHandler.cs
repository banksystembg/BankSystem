namespace CentralApi.Infrastructure.Handlers
{
    using BankSystem.Common;
    using BankSystem.Common.Utils;
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Security.Cryptography;
    using System.Threading;
    using System.Threading.Tasks;

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
            using (var rsa = RSA.Create())
            {
                RsaExtensions.FromXmlString(rsa, this.apiSigningKey);
                var aesParams = CryptographyExtensions.GenerateKey();
                var key = Convert.FromBase64String(aesParams[0]);
                var IV = Convert.FromBase64String(aesParams[1]);

                var content = await request.Content.ReadAsByteArrayAsync();
                var signedData = rsa.SignData(content, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                var requestSignedData = CryptographyExtensions.Encrypt(Convert.ToBase64String(signedData), key, IV);

                string encryptedKey;
                string encryptedIV;
                using (var encryptionRsa = RSA.Create())
                {
                    RsaExtensions.FromXmlString(encryptionRsa, this.bankKey);
                    encryptedKey = Convert.ToBase64String(encryptionRsa.Encrypt(Convert.FromBase64String(aesParams[0]), RSAEncryptionPadding.Pkcs1));
                    encryptedIV = Convert.ToBase64String(encryptionRsa.Encrypt(Convert.FromBase64String(aesParams[1]), RSAEncryptionPadding.Pkcs1));
                }

                //Setting the values in the Authorization header using custom scheme (bsw)
                request.Headers.Authorization = new AuthenticationHeaderValue(GlobalConstants.AuthenticationScheme,
                    $"{encryptedKey},{encryptedIV},{Convert.ToBase64String(requestSignedData)}");

                var response = await base.SendAsync(request, cancellationToken);
                return response;
            }
        }
    }
}
