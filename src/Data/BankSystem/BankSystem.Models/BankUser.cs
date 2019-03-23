namespace BankSystem.Models
{
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Identity;

    public class BankUser : IdentityUser
    {
        public string FullName { get; set; }

        public ICollection<BankAccount> BankAccounts { get; set; }

        public ICollection<Card> Cards { get; set; }
    }
}
