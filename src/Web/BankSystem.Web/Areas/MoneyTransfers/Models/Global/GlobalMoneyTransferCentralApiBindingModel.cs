namespace BankSystem.Web.Areas.MoneyTransfers.Models.Global
{
    using System.ComponentModel.DataAnnotations;
    using Common;
    using Common.AutoMapping.Interfaces;
    using Create;
    using Services.Models.MoneyTransfer;

    public class GlobalMoneyTransferCentralApiBindingModel : IMapWith<GlobalMoneyTransferCreateBindingModel>,
        IMapWith<MoneyTransferCreateServiceModel>
    {
        [Required]
        [MaxLength(ModelConstants.BankAccount.SwiftCodeMaxLength)]
        public string DestinationBankSwiftCode { get; set; }

        [Required]
        [MaxLength(ModelConstants.BankAccount.NameMaxLength)]
        public string DestinationBankName { get; set; }

        [Required]
        [MaxLength(ModelConstants.BankAccount.CountryMaxLength)]
        public string DestinationBankCountry { get; set; }

        [Required]
        [MaxLength(ModelConstants.BankAccount.UniqueIdMaxLength)]
        public string DestinationBankAccountUniqueId { get; set; }

        [MaxLength(ModelConstants.MoneyTransfer.DescriptionMaxLength)]
        public string Description { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        [MaxLength(ModelConstants.User.FullNameMaxLength)]
        public string SenderName { get; set; }

        [Required]
        [MaxLength(ModelConstants.BankAccount.UniqueIdMaxLength)]
        public string SenderAccountUniqueId { get; set; }
    }
}