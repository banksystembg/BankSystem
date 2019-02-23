namespace BankSystem.Web.Models.ForeignMoneyTransfers.Create
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using MoneyTransfers;

    public class ForeignMoneyTransferCreateBindingModel : IMoneyTransferCreateBindingModel
    {
        [Required]
        public ForeignMoneyTransferCreateDestinationBankDto DestinationBank { get; set; }

        [MaxLength(150)]
        [Display(Name = "Details of Payment")]
        public string Description { get; set; }

        [Required]
        [Range(typeof(decimal), "0.01", "79228162514264337593543950335", ErrorMessage = "The Amount field cannot be lower than 0.01")]
        public decimal Amount { get; set; }

        [Required]
        [Display(Name = "From Account")]
        public string AccountId { get; set; }
        
        [Display(Name = "Name")]
        public string SenderName { get; set; }

        public IEnumerable<SelectListItem> UserAccounts { get; set; }
    }
}