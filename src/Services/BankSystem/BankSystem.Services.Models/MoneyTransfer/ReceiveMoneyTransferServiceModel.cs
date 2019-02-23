namespace BankSystem.Services.Models.ForeignMoneyTransfer
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class ReceiveMoneyTransferServiceModel : MoneyTransferBaseServiceModel
    {
        public string Description { get; set; }

        [Required]
        [Range(typeof(decimal), "0.01", "79228162514264337593543950335")]
        public decimal Amount { get; set; }

        [Required]
        public string AccountId { get; set; }

        [Required]
        public DateTime MadeOn { get; set; } = DateTime.UtcNow;

        [Required]
        public string Source => this.Destination;

        [Required]
        public string Destination { get; set; }
    }
}
