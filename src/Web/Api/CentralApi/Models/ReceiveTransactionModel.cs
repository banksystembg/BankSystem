namespace CentralApi.Models
{
    using System.ComponentModel.DataAnnotations;

    public class ReceiveTransactionModel
    {
        [Required]
        [MaxLength(11)]
        public string DestinationBankSwiftCode { get; set; }

        [Required]
        [MaxLength(35)]
        public string DestinationBankName { get; set; }

        [Required]
        [MaxLength(35)]
        public string DestinationBankCountry { get; set; }

        [Required]
        [MaxLength(34)]
        public string DestinationBankAccountUniqueId { get; set; }
        
        [MaxLength(150)]
        public string Description { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        [MaxLength(35)]
        public string SenderName { get; set; }

        [Required]
        [MaxLength(34)]
        public string SenderAccountUniqueId { get; set; }
    }
}
