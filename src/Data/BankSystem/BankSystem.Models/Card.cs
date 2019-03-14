namespace BankSystem.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class Card
    {
        public string Id { get; set; }

        [Required]
        public string Number { get; set; }

        [Required]
        public string Name { get; set; }

        public DateTime ExpiryDate { get; set; } = DateTime.UtcNow.AddYears(4);

        [Required]
        public int SecurityCode { get; set; }

        [Required]
        public string UserId { get; set; }

        public BankUser User { get; set; }

        [Required]
        public string AccountId { get; set; }

        public BankAccount Account { get; set; }
    }
}
