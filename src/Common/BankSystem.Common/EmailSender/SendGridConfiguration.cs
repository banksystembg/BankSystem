namespace BankSystem.Common.EmailSender
{
    using System.ComponentModel.DataAnnotations;

    public class SendGridConfiguration
    {
        [Required]
        public string ApiKey { get; set; }
    }
}