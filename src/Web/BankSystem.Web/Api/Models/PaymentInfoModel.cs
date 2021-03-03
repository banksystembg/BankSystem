namespace BankSystem.Web.Api.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Common.AutoMapping.Interfaces;
    using Infrastructure.Helpers.GlobalTransferHelpers.Models;

    public class PaymentInfoModel : IMapWith<GlobalTransferDto>
    {
        [Required]
        [Range(typeof(decimal), "0.1", "79228162514264337593543950335")]
        public decimal Amount { get; set; }

        public string Description { get; set; }

        [Required] public string Number { get; set; }

        [Required] public string Name { get; set; }

        [Required] public string RecipientName { get; set; }

        [Required] public string ExpiryDate { get; set; }

        [Required] public string SecurityCode { get; set; }

        [Required] public string DestinationBankName { get; set; }

        [Required] public string DestinationBankSwiftCode { get; set; }

        [Required] public string DestinationBankCountry { get; set; }

        [Required] public string DestinationBankAccountUniqueId { get; set; }
    }
}