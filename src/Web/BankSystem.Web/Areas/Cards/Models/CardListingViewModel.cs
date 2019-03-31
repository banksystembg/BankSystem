namespace BankSystem.Web.Areas.Cards.Models
{
    using Infrastructure.Collections;

    public class CardListingViewModel
    {
        public PaginatedList<CardListingDto> Cards { get; set; }
    }
}