namespace BankSystem.Common.Utils
{
    using System;
    using System.Security.Cryptography;
    using System.Text;

    public static class SignatureVerificationUtil
    {
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

                return isVerified ? decrypted : null;
            }
        }
    }
}