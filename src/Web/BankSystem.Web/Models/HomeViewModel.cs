namespace BankSystem.Web.Models
{
    using System.Collections.Generic;
    using BankAccount;

    public class HomeViewModel
    {
        public IEnumerable<BankAccountIndexViewModel> UserBankAccounts { get; set; }
    }
}
