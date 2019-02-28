namespace BankSystem.Services.Models.MoneyTransfer
{
    using System;

    public class MoneyTransferListingServiceModel : MoneyTransferBaseServiceModel
    {
        public string Id { get; set; }

        public string Description { get; set; }
        
        public decimal Amount { get; set; }

        public string AccountUserFullName { get; set; }

        public string AccountName { get; set; }

        public DateTime MadeOn { get; set; }
        
        public string Source { get; set; }

        public string Destination { get; set; }
    }
}
