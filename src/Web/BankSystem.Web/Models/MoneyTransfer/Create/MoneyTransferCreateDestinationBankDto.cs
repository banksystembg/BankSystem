namespace BankSystem.Web.Models.MoneyTransfer.Create
{
    using System.ComponentModel.DataAnnotations;

    public class MoneyTransferCreateDestinationBankDto
    {
        [Required]
        [Display(Name = "Swift/Bank code")]
        public string SwiftCode { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Country { get; set; }

        [Required]
        public MoneyTransferCreateDestinationAccountDto Account { get; set; }
    }
}
