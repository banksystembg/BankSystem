namespace PaymentsDemo.Models
{
    public class CreateDemoPaymentBindingModel
    {
        public decimal Amount { get; set; }

        public string Description { get; set; }

        public string DestinationBankName { get; set; }

        public string DestinationBankSwiftCode { get; set; }

        public string DestinationBankCountry { get; set; }

        public string DestinationBankAccountUniqueId { get; set; }

        public string RecipientName { get; set; }
    }
}