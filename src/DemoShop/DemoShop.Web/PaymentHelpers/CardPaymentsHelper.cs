namespace DemoShop.Web.PaymentHelpers
{
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Models;

    public static class CardPaymentsHelper
    {
        public static async Task<bool> ProcessPaymentAsync(CardPaymentSubmitModel model, string centralApiSubmitUrl)
        {
            var client = new HttpClient();
            var request = await client.PostAsJsonAsync(centralApiSubmitUrl, model);

            return request.StatusCode == HttpStatusCode.OK;
        }
    }
}