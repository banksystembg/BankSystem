namespace BankSystem.Web.Api.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class PaymentInfoModel
    {
        [Required]
        public decimal Amount { get; set; }

        public string Description { get; set; }

        [Required]
        public string Number { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string ExpiryDate { get; set; }

        public DateTime ParsedExpiryDate =>
            DateTime.Parse($"{DateTime.DaysInMonth(int.Parse(this.ExpiryDate.Split('/')[1]), int.Parse(this.ExpiryDate.Split('/')[0]))}/{this.ExpiryDate}");

        [Required]
        public int SecurityCode { get; set; }

        [Required]
        public string DestinationBankName { get; set; }

        [Required]
        public string DestinationBankSwiftCode { get; set; }

        [Required]
        public string DestinationBankCountry { get; set; }

        [Required]
        public string DestinationBankAccountUniqueId { get; set; }
    }
}
