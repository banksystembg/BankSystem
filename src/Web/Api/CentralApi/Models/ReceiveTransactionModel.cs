namespace CentralApi.Models
{
    using System.ComponentModel.DataAnnotations;

    public class ReceiveTransactionModel
    {
        // Which account sends the money
        [Required]
        public string AccountId { get; set; }

        // To which account the money should be send
        [Required]
        public string Destination { get; set; }

        [Required]
        public string DestinationBankSwiftCode { get; set; }

        [Required]
        public string DestinationBankName { get; set; }

        [Required]
        public string DestinationBankCountry { get; set; }

        public string Description { get; set; }

        [Required]
        public decimal Amount { get; set; }

    }
}
