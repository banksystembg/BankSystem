namespace CentralApi.Services.Models.Banks
{
    public class BankPaymentServiceModel : BankBaseServiceModel
    {
        public string ApiKey { get; set; }

        public string PaymentUrl { get; set; }

        public string CardPaymentUrl { get; set; }
    }
}