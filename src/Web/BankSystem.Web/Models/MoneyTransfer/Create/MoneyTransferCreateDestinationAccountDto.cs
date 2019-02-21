namespace BankSystem.Web.Models.MoneyTransfer.Create
{
    using System.ComponentModel.DataAnnotations;

    public class MoneyTransferCreateDestinationAccountDto
    {
        [Required]
        [Display(Name = "Account/IBAN")]
        public string Id { get; set; }

        [Required]
        [Display(Name = "Beneficiary's name")]
        public string UserFullName { get; set; }
    }
}
