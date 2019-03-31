namespace BankSystem.Web.Api.Models
{
    public class ReceiveCardPaymentModel
    {
        public string PaymentInfo { get; set; }

        public string PaymentInfoSignature { get; set; }

        public string PaymentProof { get; set; }

        public string PaymentProofSignature { get; set; }
    }
}