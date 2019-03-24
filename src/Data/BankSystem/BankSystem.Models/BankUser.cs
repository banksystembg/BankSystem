namespace BankSystem.Models
{
    using Microsoft.AspNetCore.Identity;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Common;

    public class BankUser : IdentityUser
    {
        [Required]
        [MaxLength(ModelConstants.User.FullNameMaxLength)]
        public string FullName { get; set; }

        public ICollection<BankAccount> BankAccounts { get; set; }

        public ICollection<Card> Cards { get; set; }
    }
}
