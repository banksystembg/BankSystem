namespace DemoShop.Web.Models
{
    public class PaymentPostRedirectModel
    {
        public string Url { get; set; }

        public string PaymentDataFormKey { get; set; }

        public string PaymentData { get; set; }
    }
}