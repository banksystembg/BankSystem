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

                var serializedModel = JsonConvert.SerializeObject(model);
                var dataObject = new
                {
                    Model = serializedModel,
                    Timestamp = DateTime.UtcNow
                };

                var data = JsonConvert.SerializeObject(dataObject);

                var signature = Convert.ToBase64String(rsa
                    .SignData(Encoding.UTF8.GetBytes(data), HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1));

                // Encrypt with bank public key
                string encryptedKey;
                string encryptedIv;
                using (var encryptionRsa = RSA.Create())
                {
                    RsaExtensions.FromXmlString(encryptionRsa, bankKey);
                    encryptedKey = Convert.ToBase64String(encryptionRsa.Encrypt(key, RSAEncryptionPadding.Pkcs1));
                    encryptedIv = Convert.ToBase64String(encryptionRsa.Encrypt(iv, RSAEncryptionPadding.Pkcs1));
                }

                var encryptedData = Convert.ToBase64String(CryptographyExtensions.Encrypt(data, key, iv));

                var json = new
                {
                    EncryptedKey = encryptedKey,
                    EncryptedIv = encryptedIv,
                    Data = encryptedData,
                    Signature = signature
                };

                var serializedJson = JsonConvert.SerializeObject(json);
                var request = Convert.ToBase64String(Encoding.UTF8.GetBytes(serializedJson));

                return request;
            }
        }
    }
}