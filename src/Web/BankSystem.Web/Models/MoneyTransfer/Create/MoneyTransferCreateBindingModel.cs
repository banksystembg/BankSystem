namespace BankSystem.Web.Models.MoneyTransfer
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Common.AutoMapping.Interfaces;
    using Create;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Services.Models.MoneyTransfer;

    public class MoneyTransferCreateBindingModel : IMapWith<MoneyTransferCreateServiceModel>
    {
        [Required]
        public MoneyTransferCreateDestinationBankDto DestinationBank { get; set; }

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
