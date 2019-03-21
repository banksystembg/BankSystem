namespace BankSystem.Services.Models.Card
{
    using System;

    public class CardListingServiceModel : CardBaseServiceModel
    {
        public string Id { get; set; }

        public string Number { get; set; }

        public string Name { get; set; }

        public DateTime ExpiryDate { get; set; }

        public int SecurityCode { get; set; }

        public string AccountName { get; set; }
    }
}
