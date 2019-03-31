namespace BankSystem.Web.Areas.MoneyTransfers.Models.Global.Create
{
    using System.ComponentModel.DataAnnotations;
    using Common;

    public class GlobalMoneyTransferCreateDestinationAccountDto
    {
        [Required]
        [MaxLength(ModelConstants.BankAccount.UniqueIdMaxLength)]
        [Display(Name = "Account/IBAN")]
        public string UniqueId { get; set; }

        [Required]
        [MaxLength(ModelConstants.User.FullNameMaxLength)]
        [Display(Name = "Beneficiary's name")]
        public string UserFullName { get; set; }
    }
}