namespace CentralApi.Models
{
    public class ReceiveTransactionModel
    {
        public string DestinationBankSwiftCode { get; set; }

        public string DestinationBankName { get; set; }

        public string DestinationBankCountry { get; set; }

        public string DestinationBankAccountUniqueId { get; set; }

        public string Description { get; set; }

        public decimal Amount { get; set; }

        public string SenderName { get; set; }

        public string SenderAccountUniqueId { get; set; }
    }
}
