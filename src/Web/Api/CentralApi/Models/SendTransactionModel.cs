namespace CentralApi.Models
{
    using System.ComponentModel.DataAnnotations;
    using BankSystem.Common.AutoMapping.Interfaces;

    public class SendTransactionModel : IMapWith<ReceiveTransactionModel>
    {
        [MaxLength(150)]
        public string Description { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public string DestinationBankAccountUniqueId { get; set; }

        [Required]
        public string SenderName { get; set; }

        [Required]
        public string SenderAccountUniqueId { get; set; }
    }
}
