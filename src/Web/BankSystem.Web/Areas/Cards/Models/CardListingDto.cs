namespace BankSystem.Web.Areas.Cards.Models
{
    using Common.AutoMapping.Interfaces;
    using Services.Models.Card;
    using System;

    public class CardListingDto : IMapWith<CardListingServiceModel>
    {
        public string Id { get; set; }

        public string Number { get; set; }

        public string Name { get; set; }

        public DateTime ExpiryDate { get; set; }

        public int SecurityCode { get; set; }

        public string AccountName { get; set; }
    }
}
