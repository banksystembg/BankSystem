namespace BankSystem.Web.Models
{
    using System.Collections.Generic;
    using BankAccount;
    using ForeignMoneyTransfers;
    using MoneyTransfers;

    public class HomeViewModel
    {
        public IEnumerable<BankAccountIndexViewModel> UserBankAccounts { get; set; }
        public IEnumerable<MoneyTransferListingViewModel> MoneyTransfers { get; set; }
    }
}
