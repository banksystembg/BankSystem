namespace BankSystem.Web.Areas.MoneyTransfers.Models
{
    using System;
    using Common.AutoMapping.Interfaces;
    using Services.Models.MoneyTransfer;

    public class MoneyTransferListingDto : IMapWith<MoneyTransferListingServiceModel>
    {
        public string Description { get; set; }

        public decimal Amount { get; set; }

        public string AccountUserFullName { get; set; }

        public string AccountName { get; set; }

        public DateTime MadeOn { get; set; }

        public string Source { get; set; }

        public string Destination { get; set; }
    }
}