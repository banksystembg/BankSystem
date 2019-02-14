namespace BankSystem.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class MoneyTransfer
    {
        public string Id { get; set; }
        
        public string Description { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public DateTime MadeOn { get; set; }

        [Required]
        public string AccountId { get; set; }

        public BankAccount Account { get; set; }

        [Required]
        public string Source { get; set; }

        [Required]
        public string Destination { get; set; }
    }
}
