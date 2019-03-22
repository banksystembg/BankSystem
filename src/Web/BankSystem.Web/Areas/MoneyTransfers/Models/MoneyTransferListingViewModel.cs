namespace BankSystem.Web.Areas.MoneyTransfers.Models
{
    using Common.AutoMapping.Interfaces;
    using Services.Models.MoneyTransfer;
    using System;

    public class MoneyTransferListingViewModel : IMapWith<MoneyTransferListingServiceModel>
    {
        public string Description { get; set; }

        public decimal Amount { get; set; }

        public string SenderName { get; set; }

        public string RecipientName { get; set; }

        public DateTime MadeOn { get; set; }

        public string Source { get; set; }

        public string Destination { get; set; }
    }
}
