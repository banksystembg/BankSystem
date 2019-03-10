namespace BankSystem.Web.Models.BankAccount
{
    using System.Collections.Generic;
    using Areas.MoneyTransfers.Models;

    public class BankAccountDetailsViewModel
    {
        public string BankAccountUniqueId { get; set; }

        public IEnumerable<MoneyTransferListingViewModel> MoneyTransfers { get; set; }
    }
}
