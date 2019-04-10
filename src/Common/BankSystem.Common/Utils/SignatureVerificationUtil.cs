namespace BankSystem.Common.Utils
{
    using System;
    using System.Globalization;
    using System.Security.Cryptography;
    using System.Text;

    public static class SignatureVerificationUtil
    {
        private const int TimestampValidityBeforeIssuanceInMinutes = 2;
        private const int TimestampValidityAfterIssuanceInMinutes = 2;

        public static string DecryptDataAndVerifySignature(
            string decryptionPrivateKey,
            string signaturePublicKey,
            string encryptedKey,
            string encryptedIv,
            string data,
            string signature)
        {
            // Decrypt
            string decrypted;
            using (var rsa = RSA.Create())
            {
                RsaExtensions.FromXmlString(rsa, decryptionPrivateKey);
                var decryptedKey = rsa.Decrypt(Convert.FromBase64String(encryptedKey), RSAEncryptionPadding.Pkcs1);
                var decryptedIv = rsa.Decrypt(Convert.FromBase64String(encryptedIv), RSAEncryptionPadding.Pkcs1);

                decrypted = CryptographyExtensions.Decrypt(Convert.FromBase64String(data), decryptedKey, decryptedIv);
            }

            // Verify
            using (var rsa = RSA.Create())
            {
                RsaExtensions.FromXmlString(rsa, signaturePublicKey);

                var decrypt = Encoding.UTF8.GetBytes(decrypted);
                var isVerified = rsa.VerifyData(decrypt, Convert.FromBase64String(signature), HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

                if (!isVerified)
                {
                    return null;
                }
            }

            // Check timestamp
            var split = decrypted.Split('\0');

            if (split.Length != 2)
            {
                return null;
            }

            var json = split[0];

            bool timestampParsed = DateTime.TryParseExact(
                split[1],
                "O",
                CultureInfo.InvariantCulture,
                DateTimeStyles.AdjustToUniversal,
                out var timestamp);

            if (!timestampParsed)
            {
                return null;
            }

            bool timestampValid = IsTimestampValid(timestamp);

            if (!timestampValid)
            {
                return null;
            }

            return json;
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