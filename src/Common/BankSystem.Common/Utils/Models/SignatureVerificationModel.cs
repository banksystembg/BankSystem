namespace BankSystem.Common.Utils.Models
{
    using System.ComponentModel.DataAnnotations;

    public class SignatureVerificationModel
    {
        [Required]
        public string DecryptionPrivateKey { get; set; }

        [Required]
        public string SignaturePublicKey { get; set; }

        [Required]
        public string EncryptedKey { get; set; }

        [Required]
        public string EncryptedIv { get; set; }

        [Required]
        public string Data { get; set; }

        [Required]
        public string Signature { get; set; }
    }
}