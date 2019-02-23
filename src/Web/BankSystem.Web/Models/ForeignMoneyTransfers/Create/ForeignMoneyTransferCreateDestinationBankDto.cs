namespace BankSystem.Web.Models.ForeignMoneyTransfers.Create
{
    using System.ComponentModel.DataAnnotations;

    public class ForeignMoneyTransferCreateDestinationBankDto
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
        public ForeignMoneyTransferCreateDestinationAccountDto Account { get; set; }
    }
}
