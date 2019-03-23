namespace BankSystem.Services.Models.Card
{
    using System.ComponentModel.DataAnnotations;
    using BankSystem.Models;
    using Common;

    public class CardCreateServiceModel : CardBaseServiceModel
    {
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
