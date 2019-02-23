namespace BankSystem.Web.Models.ForeignMoneyTransfers.Create
{
    using System.ComponentModel.DataAnnotations;

    public class ForeignMoneyTransferCreateDestinationAccountDto
    {
        [Required]
        [MaxLength(34)]
        [Display(Name = "Account/IBAN")]
        public string UniqueId { get; set; }

        [Required]
        [MaxLength(35)]
        [Display(Name = "Beneficiary's name")]
        public string UserFullName { get; set; }
    }
}
