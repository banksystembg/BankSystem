namespace BankSystem.Web.Api.Models
{
    using System.ComponentModel.DataAnnotations;
    using Common.AutoMapping.Interfaces;
    using Services.Models.GlobalTransfer;

    public class PaymentInfoModel : IMapWith<GlobalTransferServiceModel>
    {
        [Required]
        public decimal Amount { get; set; }

        public string Description { get; set; }

        [Required]
        public string Number { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string RecipientName { get; set; }

        [Required]
        public string ExpiryDate { get; set; }

        [Required]
        public int SecurityCode { get; set; }

        [Required]
        public string DestinationBankName { get; set; }

        [Required]
        public string DestinationBankSwiftCode { get; set; }

        [Required]
        public string DestinationBankCountry { get; set; }

        [Required]
        public string DestinationBankAccountUniqueId { get; set; }
    }
}
