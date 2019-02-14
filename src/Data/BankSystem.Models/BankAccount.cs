namespace BankSystem.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class BankAccount
    {
        public string Id { get; set; }

        [Required]
        public decimal Balance { get; set; }

        [Required]
        public string UniqueId { get; set; }

        [Required]
        public DateTime CreatedOn { get; set; }

        [Required]
        public string UserId { get; set; }

        public BankUser User { get; set; }

        public ICollection<MoneyTransfer> Transfers { get; set; }
    }
}
