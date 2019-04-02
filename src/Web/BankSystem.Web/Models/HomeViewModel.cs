namespace BankSystem.Web.Models
{
    using System.Collections.Generic;
    using Areas.MoneyTransfers.Models;
    using BankAccount;

    public class HomeViewModel
    {
        public IEnumerable<BankAccountIndexViewModel> UserBankAccounts { get; set; }

        public IEnumerable<MoneyTransferListingDto> MoneyTransfers { get; set; }
    }
}