namespace BankSystem.Web.Areas.Cards.Models
{
    using Common.AutoMapping.Interfaces;
    using Services.Models.Card;

    public class CardListingDto : IMapWith<CardDetailsServiceModel>
    {
        public string Id { get; set; }

        public string Number { get; set; }

        public string Name { get; set; }

        public string ExpiryDate { get; set; }

        public string SecurityCode { get; set; }

        public string AccountName { get; set; }
    }
}