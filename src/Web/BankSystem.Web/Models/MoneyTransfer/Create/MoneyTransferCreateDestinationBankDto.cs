namespace BankSystem.Web.Models.MoneyTransfer.Create
{
    using System.ComponentModel.DataAnnotations;

    public class MoneyTransferCreateDestinationBankDto
    {
        [Required]
        [MaxLength(11)]
        [Display(Name = "Swift/Bank code")]
        public string SwiftCode { get; set; }

        [Required]
        [MaxLength(35)]
        public string Name { get; set; }

        [Required]
        [MaxLength(35)]
        public string Country { get; set; }

        [Required]
        public MoneyTransferCreateDestinationAccountDto Account { get; set; }
    }
}
