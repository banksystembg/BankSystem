namespace BankSystem.Web.Areas.Cards.Models
{
    using Common.AutoMapping.Interfaces;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Services.Models.Card;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class CardCreateViewModel : IMapWith<CardCreateServiceModel>
    {
        public IEnumerable<SelectListItem> BankAccounts { get; set; }

        [Required]
        [Display(Name = "Choose account")]
        public string AccountId { get; set; }
    }
}
