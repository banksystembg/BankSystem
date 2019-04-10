namespace BankSystem.Common.Utils
{
    using Models;
    using Newtonsoft.Json;
    using System;
    using System.Globalization;
    using System.Security.Cryptography;
    using System.Text;

    public static class SignatureVerificationUtil
    {
        private const int TimestampValidityBeforeIssuanceInMinutes = 2;
        private const int TimestampValidityAfterIssuanceInMinutes = 2;

        public static string DecryptDataAndVerifySignature(SignatureVerificationModel model)
        {
            if (!ValidationUtil.IsObjectValid(model))
            {
                return null;
            }

            // Decrypt
            string decrypted;
            using (var rsa = RSA.Create())
            {
                RsaExtensions.FromXmlString(rsa, model.DecryptionPrivateKey);
                var decryptedKey = rsa.Decrypt(Convert.FromBase64String(model.EncryptedKey),
                    RSAEncryptionPadding.Pkcs1);

                var decryptedIv = rsa.Decrypt(Convert.FromBase64String(model.EncryptedIv),
                    RSAEncryptionPadding.Pkcs1);

                decrypted = CryptographyExtensions.Decrypt(Convert.FromBase64String(model.Data), decryptedKey,
                    decryptedIv);
            }

            // Verify
            using (var rsa = RSA.Create())
            {
                RsaExtensions.FromXmlString(rsa, model.SignaturePublicKey);

                var decrypt = Encoding.UTF8.GetBytes(decrypted);
                var isVerified = rsa.VerifyData(decrypt, Convert.FromBase64String(model.Signature),
                    HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

                if (!isVerified)
                {
                    return null;
                }
            }

            dynamic dataObject = JsonConvert.DeserializeObject(decrypted);
            string json = dataObject.Model;

            // Check timestamp
            DateTime timestamp = dataObject.Timestamp;
            if (timestamp.Kind != DateTimeKind.Utc)
            {
                bool isParsed = DateTime.TryParseExact(
                    timestamp.ToString(GlobalConstants.DateTimeFormat, CultureInfo.InvariantCulture),
                    GlobalConstants.DateTimeFormat,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AdjustToUniversal,
                    out timestamp);

                if (!isParsed)
                {
                    return null;
                }
            }

            bool timestampValid = IsTimestampValid(timestamp);
            return timestampValid ? json : null;
        }

        private static bool IsTimestampValid(DateTime timestamp)
        {
            var currentTime = DateTime.UtcNow;

            bool timestampValid = currentTime.AddMinutes(-TimestampValidityBeforeIssuanceInMinutes) < timestamp
                                  && timestamp < currentTime.AddMinutes(TimestampValidityAfterIssuanceInMinutes);

            return timestampValid;
        }
    }
}