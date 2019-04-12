namespace BankSystem.Web.Areas.Administration.Models
{
    using System.Collections.Generic;
    using MoneyTransfers.Models;

    public class TransactionListingViewModel
    {
        public IEnumerable<MoneyTransferListingDto> MoneyTransfers { get; set; }

        public string ReferenceNumber { get; set; }
    }
}
