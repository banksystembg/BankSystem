namespace BankSystem.Web.Models.MoneyTransfer
{
    using System.ComponentModel.DataAnnotations;
    using Common.AutoMapping.Interfaces;

    public class MoneyTransferCentralApiBindingModel : IMapWith<MoneyTransferCreateBindingModel>
    {
        [Required]
        public string DestinationBankSwiftCode { get; set; }

        [Required]
        public string DestinationBankName { get; set; }

        [Required]
        public string DestinationBankCountry { get; set; }

        [Required]
        public string Destination { get; set; }
        
        public string Description { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public string AccountId { get; set; }
    }
}
