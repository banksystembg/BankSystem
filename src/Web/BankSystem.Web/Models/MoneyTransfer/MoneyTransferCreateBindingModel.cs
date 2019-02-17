namespace BankSystem.Web.Models.MoneyTransfer
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Common.AutoMapping.Interfaces;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Services.Models.MoneyTransfer;

    public class MoneyTransferCreateBindingModel : IMapWith<MoneyTransferCreateServiceModel>
    {
        [Required]
        [Display(Name = "Destination account")]
        public string Destination { get; set; }

        [Required]
        [Display(Name = "Details of Payment")]
        public string Description { get; set; }

        [Required]
        [Range(typeof(decimal), "0.01", "79228162514264337593543950335")]
        public decimal Amount { get; set; }

        [Required]
        [Display(Name = "From Account")]
        public string AccountId { get; set; }
        
        public IEnumerable<SelectListItem> UserAccounts { get; set; }
    }
}
