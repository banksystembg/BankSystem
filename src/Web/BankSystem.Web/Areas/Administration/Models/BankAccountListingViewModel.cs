namespace BankSystem.Web.Areas.Administration.Models
{
    using System;
    using Common.AutoMapping.Interfaces;
    using Services.Models.BankAccount;

    public class BankAccountListingViewModel : IMapWith<BankAccountDetailsServiceModel>
    {
        public string Name { get; set; }

        public decimal Balance { get; set; }

        public string UniqueId { get; set; }

        public DateTime CreatedOn { get; set; }

        public string UserUserName { get; set; }

        public string UserFullName { get; set; }
    }
}