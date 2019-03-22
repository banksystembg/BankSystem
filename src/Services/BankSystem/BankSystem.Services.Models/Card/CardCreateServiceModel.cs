namespace BankSystem.Services.Models.Card
{
    using BankSystem.Models;
    using System.ComponentModel.DataAnnotations;

    public class CardCreateServiceModel : CardBaseServiceModel
    {
        public string Number { get; set; }

        public string Name { get; set; }

        public string ExpiryDate { get; set; }

        public int SecurityCode { get; set; }

        [Required]
        public string UserId { get; set; }

        public BankUser User { get; set; }

        [Required]
        public string AccountId { get; set; }

        public BankAccount Account { get; set; }
    }
}
