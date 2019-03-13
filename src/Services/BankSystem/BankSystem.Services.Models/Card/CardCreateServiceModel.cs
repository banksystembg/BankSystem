namespace BankSystem.Services.Models.Card
{
    using BankSystem.Models;
    using System;
    using System.ComponentModel.DataAnnotations;

    public class CardCreateServiceModel : CardBaseServiceModel
    {
        public int Number { get; set; }

        public string Name { get; set; }

        public DateTime ExpiryDate { get; set; } = DateTime.UtcNow.AddYears(4);

        public int SecurityCode { get; set; }

        [Required]
        public string UserId { get; set; }

        public BankUser User { get; set; }

        [Required]
        public string AccountId { get; set; }

        public BankAccount Account { get; set; }
    }
}
