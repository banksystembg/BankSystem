namespace CentralApi.Models
{
    public class CardPaymentReceiveModel
    {
        public string PaymentInfo { get; set; }

        public string PaymentInfoSignature { get; set; }

        public string PublicKey { get; set; }
    }
}
