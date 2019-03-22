namespace BankSystem.Models
{
    using System.ComponentModel.DataAnnotations;

    public class Card
    {
        public string Id { get; set; }

        [Required]
        public string Number { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [StringLength(5, MinimumLength = 5)]
        public string ExpiryDate { get; set; }

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
