namespace BankSystem.Web.Areas.Cards.Models
{
    using Microsoft.AspNetCore.Mvc.Rendering;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class CardCreateViewModel
    {
        public IEnumerable<SelectListItem> BankAccounts { get; set; }

        [Required]
        [Display(Name = "Choose account")]
        public string BankAccountId { get; set; }
    }
}
