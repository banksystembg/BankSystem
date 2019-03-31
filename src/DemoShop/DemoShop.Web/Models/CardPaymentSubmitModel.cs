namespace DemoShop.Web.Models
{
    public class CardPaymentSubmitModel
    {
        public string Number { get; set; }

        public string Name { get; set; }

        public string ExpiryDate { get; set; }

        public string SecurityCode { get; set; }

        public decimal Amount { get; set; }

        public string Description { get; set; }

        public string DestinationBankName { get; set; }

        public string DestinationBankCountry { get; set; }

        public string DestinationBankSwiftCode { get; set; }

        public string DestinationBankAccountUniqueId { get; set; }

        public string RecipientName { get; set; }
    }
}