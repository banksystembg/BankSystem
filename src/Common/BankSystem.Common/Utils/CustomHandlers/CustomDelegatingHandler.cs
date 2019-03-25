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
        private readonly string bankCountry;

        public CustomDelegatingHandler(string apiSigningKey, string bankKey, string bankName, string bankSwiftCode, string bankCountry)
        {
            this.apiSigningKey = apiSigningKey;
            this.bankKey = bankKey;
            this.bankName = bankName;
            this.bankSwiftCode = bankSwiftCode;
            this.bankCountry = bankCountry;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            using (var rsa = RSA.Create())
            {
                RsaExtensions.FromXmlString(rsa, this.bankKey);
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
                    RsaExtensions.FromXmlString(encryptionRsa, this.apiSigningKey);
                    encryptedKey = Convert.ToBase64String(encryptionRsa.Encrypt(Convert.FromBase64String(aesParams[0]), RSAEncryptionPadding.Pkcs1));
                    encryptedIV = Convert.ToBase64String(encryptionRsa.Encrypt(Convert.FromBase64String(aesParams[1]), RSAEncryptionPadding.Pkcs1));
                }

                //Setting the values in the Authorization header using custom scheme (bsw)
                request.Headers.Authorization = new AuthenticationHeaderValue(GlobalConstants.AuthenticationScheme,
                    $"{this.bankName},{this.bankSwiftCode},{this.bankCountry},{encryptedKey},{encryptedIV},{Convert.ToBase64String(requestSignedData)}");

                var response = await base.SendAsync(request, cancellationToken);
                return response;
            }
        }
    }
}
