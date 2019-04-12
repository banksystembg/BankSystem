namespace BankSystem.Web.Api.Models
{
    using System.ComponentModel.DataAnnotations;

    public class ReceiveMoneyTransferModel
    {
        public string Description { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public string DestinationBankAccountUniqueId { get; set; }

        [Required]
        public string SenderName { get; set; }

        [Required]
        public string RecipientName { get; set; }

        [Required]
        public string SenderAccountUniqueId { get; set; }

        [Required]
        public string ReferenceNumber { get; set; }
    }
}