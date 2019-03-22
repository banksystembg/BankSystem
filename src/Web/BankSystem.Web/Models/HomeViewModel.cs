namespace BankSystem.Web.Models
{
    using Areas.MoneyTransfers.Models;
    using BankAccount;
    using System.Collections.Generic;

    public class HomeViewModel
    {
        public IEnumerable<BankAccountIndexViewModel> UserBankAccounts { get; set; }
        public IEnumerable<MoneyTransferListingDto> MoneyTransfers { get; set; }
    }
}
