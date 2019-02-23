namespace BankSystem.Web.Models.ForeignMoneyTransfers
{
    using System.ComponentModel.DataAnnotations;
    using Common.AutoMapping.Interfaces;
    using Create;

    public class ForeignMoneyTransferCentralApiBindingModel : IMapWith<ForeignMoneyTransferCreateBindingModel>
    {
        [Required]
        public string DestinationBankSwiftCode { get; set; }

        [Required]
        public string DestinationBankName { get; set; }

        [Required]
        public string DestinationBankCountry { get; set; }

        [Required]
        public string DestinationBankAccountUniqueId { get; set; }
        
        public string Description { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public string SenderName { get; set; }

        [Required]
        public string SenderAccountUniqueId { get; set; }
    }
}
