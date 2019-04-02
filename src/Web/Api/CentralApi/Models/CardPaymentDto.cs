namespace CentralApi.Models
{
    using System.ComponentModel.DataAnnotations;
    using BankSystem.Common;

    public class CardPaymentDto
    {
        [Required]
        public decimal Amount { get; set; }

        [MaxLength(ModelConstants.MoneyTransfer.DescriptionMaxLength)]
        public string Description { get; set; }

        [Required]
        public string Number { get; set; }

        [Required]
        [MaxLength(ModelConstants.User.FullNameMaxLength)]
        public string Name { get; set; }

        [Required]
        [MaxLength(ModelConstants.User.FullNameMaxLength)]
        public string RecipientName { get; set; }

        [Required]
        [StringLength(ModelConstants.Card.ExpiryDateMaxLength, MinimumLength = ModelConstants.Card.ExpiryDateMaxLength)]
        public string ExpiryDate { get; set; }

        [Required]
        [StringLength(ModelConstants.Card.SecurityCodeMaxLength, MinimumLength = ModelConstants.Card.SecurityCodeMaxLength)]
        public string SecurityCode { get; set; }

        [Required]
        [MaxLength(ModelConstants.BankAccount.NameMaxLength)]
        public string DestinationBankName { get; set; }

        [Required]
        [MaxLength(ModelConstants.BankAccount.SwiftCodeMaxLength)]
        public string DestinationBankSwiftCode { get; set; }

        [Required]
        [MaxLength(ModelConstants.BankAccount.CountryMaxLength)]
        public string DestinationBankCountry { get; set; }

        [Required]
        [MaxLength(ModelConstants.BankAccount.UniqueIdMaxLength)]
        public string DestinationBankAccountUniqueId { get; set; }
    }
}