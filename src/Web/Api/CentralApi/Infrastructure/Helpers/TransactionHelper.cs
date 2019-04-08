namespace CentralApi.Infrastructure.Helpers
{
    using System;
    using System.Security.Cryptography;
    using System.Text;
    using BankSystem.Common.Utils;
    using Newtonsoft.Json;

    public static class TransactionHelper
    {
        public static string SignAndEncryptData<T>(
            T model,
            string apiSigningKey,
            string bankKey)
            where T : class
        {
            // Sign data with api private key
            using (var rsa = RSA.Create())
            {
                RsaExtensions.FromXmlString(rsa, apiSigningKey);
                var aesParams = CryptographyExtensions.GenerateKey();
                var key = Convert.FromBase64String(aesParams[0]);
                var iv = Convert.FromBase64String(aesParams[1]);

                var signature = Convert.ToBase64String(rsa
                    .SignData(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(model)), HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1));

                // Encrypt with bank public key
                string encryptedKey;
                string encryptedIv;
                using (var encryptionRsa = RSA.Create())
                {
                    RsaExtensions.FromXmlString(encryptionRsa, bankKey);
                    encryptedKey = Convert.ToBase64String(encryptionRsa.Encrypt(Convert.FromBase64String(aesParams[0]), RSAEncryptionPadding.Pkcs1));
                    encryptedIv = Convert.ToBase64String(encryptionRsa.Encrypt(Convert.FromBase64String(aesParams[1]), RSAEncryptionPadding.Pkcs1));
                }

                var json = new
                {
                    EncryptedKey = encryptedKey,
                    EncryptedIv = encryptedIv,
                    Data = Convert.ToBase64String(CryptographyExtensions.Encrypt(JsonConvert.SerializeObject(model), key, iv)),
                    Signature = signature
                };

                var serializedJson = JsonConvert.SerializeObject(json);
                var encryptedData = Convert.ToBase64String(Encoding.UTF8.GetBytes(serializedJson));

                return encryptedData;
            }
        }
    }
}