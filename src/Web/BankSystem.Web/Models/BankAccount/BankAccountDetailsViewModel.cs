namespace BankSystem.Web.Models.BankAccount
{
    using Areas.MoneyTransfers.Models;
    using System.Collections.Generic;

    public class BankAccountDetailsViewModel
    {
        public string BankAccountUniqueId { get; set; }

        public IEnumerable<MoneyTransferListingDto> MoneyTransfers { get; set; }
    }
}
