namespace BankSystem.Web.Areas.MoneyTransfers.Models.Global.Create
{
    using System.ComponentModel.DataAnnotations;
    using Common;

    public class GlobalMoneyTransferCreateDestinationBankDto
    {
        [Required]
        [MaxLength(ModelConstants.BankAccount.SwiftCodeMaxLength)]
        [Display(Name = "Swift/Bank code")]
        public string SwiftCode { get; set; }

        [Required]
        [MaxLength(ModelConstants.BankAccount.NameMaxLength)]
        public string Name { get; set; }

        [Required]
        [MaxLength(ModelConstants.BankAccount.CountryMaxLength)]
        public string Country { get; set; }

        [Required]
        public GlobalMoneyTransferCreateDestinationAccountDto Account { get; set; }
    }
}