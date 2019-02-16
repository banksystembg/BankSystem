namespace BankSystem.Services.Models.MoneyTransfer
{
    using System;

    public class MoneyTransferListingServiceModel : MoneyTransferBaseServiceModel
    {
        public string Description { get; set; }
        
        public decimal Amount { get; set; }
        
        public DateTime MadeOn { get; set; }
        
        public string Destination { get; set; }
    }
}
