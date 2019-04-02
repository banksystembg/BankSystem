namespace BankSystem.Models
{
    using System.ComponentModel.DataAnnotations;
    using Common;

    public class Card
    {
        public string Id { get; set; }

        [Required]
        public string Number { get; set; }

        [Required]
        [MaxLength(ModelConstants.Card.NameMaxLength)]
        public string Name { get; set; }

        [Required]
        [StringLength(ModelConstants.Card.ExpiryDateMaxLength, MinimumLength = ModelConstants.Card.ExpiryDateMaxLength)]
        public string ExpiryDate { get; set; }

        [Required]
        [StringLength(ModelConstants.Card.SecurityCodeMaxLength, MinimumLength = ModelConstants.Card.SecurityCodeMaxLength)]
        public string SecurityCode { get; set; }

        [Required]
        public string UserId { get; set; }

        public BankUser User { get; set; }

        [Required]
        public string AccountId { get; set; }

        public BankAccount Account { get; set; }
    }
}