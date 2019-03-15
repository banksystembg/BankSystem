namespace PaymentsDemo.Models
{
    using System.ComponentModel.DataAnnotations;

    public class CreateDemoCardPaymentBindingModel
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

        [Required]
        public int SecurityCode { get; set; }

        public string DestinationBankName { get; set; }

        public string DestinationBankSwiftCode { get; set; }

        public string DestinationBankCountry { get; set; }

        public string DestinationBankAccountUniqueId { get; set; }
    }
}