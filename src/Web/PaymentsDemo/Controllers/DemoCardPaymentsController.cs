namespace PaymentsDemo.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;

    public class DemoCardPaymentsController : Controller
    {
        private const string CentralApiUrl = "https://localhost:5001/api/cardPayments";

        [HttpPost]
        public async Task<IActionResult> CreatePaymentRequest(CreateDemoCardPaymentBindingModel model)
        {
            var client = new HttpClient();
            var request = await client.PostAsJsonAsync(CentralApiUrl, model);
            if (request.StatusCode != HttpStatusCode.OK)
            {
                // Read response from the request content and visualize error message to the user
            }

            return this.RedirectToAction(nameof(this.ReceivePaymentConfirmation));
        }

        public IActionResult ReceivePaymentConfirmation()
        {
            // I'm visualizing always success message because we're going to refactor this after 2 days when we implement the online magazine
            return this.View();
        }
    }
}
