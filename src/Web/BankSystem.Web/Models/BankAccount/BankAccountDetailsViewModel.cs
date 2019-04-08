namespace BankSystem.Web.Models.BankAccount
{
    using System;
    using Areas.MoneyTransfers.Models;
    using Common.AutoMapping.Interfaces;
    using Infrastructure.Collections;
    using Services.Models.BankAccount;

    public class BankAccountDetailsViewModel : IMapWith<BankAccountDetailsServiceModel>
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public decimal Balance { get; set; }

        public string UniqueId { get; set; }

        public DateTime CreatedOn { get; set; }

        public string UserFullName { get; set; }

        public PaginatedList<MoneyTransferListingDto> MoneyTransfers { get; set; }

        public int MoneyTransfersCount { get; set; }

        public string BankName { get; set; }

        public string BankCode { get; set; }

        public string BankCountry { get; set; }
    }
}