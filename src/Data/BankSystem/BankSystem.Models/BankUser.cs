namespace BankSystem.Models
{
    using Microsoft.AspNetCore.Identity;
    using System.Collections.Generic;

    public class BankUser : IdentityUser
    {
        public string FullName { get; set; }

        public ICollection<BankAccount> BankAccounts { get; set; }

        public ICollection<Card> Cards { get; set; }
    }
}
