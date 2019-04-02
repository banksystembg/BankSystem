namespace DemoShop.Web.Controllers
{
    using System.Threading.Tasks;
    using Configuration;
    using DemoShop.Models;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using Models;
    using PaymentHelpers;
    using Services.Interfaces;

    public class CardPaymentsController : Controller
    {
        private const string PaymentUnsuccessfulError = "Payment failed. Please check your card details and try again.";
        private readonly CardPaymentsConfiguration cardPaymentsConfiguration;
        private readonly DestinationBankAccountConfiguration destinationBankAccountConfiguration;

        private readonly IOrdersService ordersService;

        public CardPaymentsController(
            IOrdersService ordersService,
            IOptions<CardPaymentsConfiguration> cardPaymentsConfigurationOptions,
            IOptions<DestinationBankAccountConfiguration> destinationBankAccountConfigurationOptions)
        {
            this.ordersService = ordersService;
            this.cardPaymentsConfiguration = cardPaymentsConfigurationOptions.Value;
            this.destinationBankAccountConfiguration = destinationBankAccountConfigurationOptions.Value;
        }

        public async Task<IActionResult> Pay(string id)
        {
            var order = await this.ordersService.GetByIdAsync(id);

            if (order == null ||
                order.UserName != this.User.Identity.Name ||
                order.PaymentStatus != PaymentStatus.Pending)
            {
                return this.RedirectToAction("My", "Orders");
            }

            var model = new CardPaymentBindingModel
            {
                ProductName = order.ProductName,
                ProductPrice = order.ProductPrice
            };

            return this.View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Pay(string id, CardPaymentBindingModel model)
        {
            var order = await this.ordersService.GetByIdAsync(id);

            if (order == null ||
                order.UserName != this.User.Identity.Name ||
                order.PaymentStatus != PaymentStatus.Pending)
            {
                return this.RedirectToAction("My", "Orders");
            }

            if (!this.ModelState.IsValid)
            {
                model.ProductName = order.ProductName;
                model.ProductPrice = order.ProductPrice;

                return this.View(model);
            }

            // create model to send to the CentralApi
            var submitModel = new CardPaymentSubmitModel
            {
                Number = model.Number,
                Name = model.Name,
                ExpiryDate = model.ExpiryDate,
                SecurityCode = model.SecurityCode,
                Amount = order.ProductPrice,
                Description = order.ProductName,
                DestinationBankName = this.destinationBankAccountConfiguration.DestinationBankName,
                DestinationBankCountry = this.destinationBankAccountConfiguration.DestinationBankCountry,
                DestinationBankSwiftCode = this.destinationBankAccountConfiguration.DestinationBankSwiftCode,
                DestinationBankAccountUniqueId =
                    this.destinationBankAccountConfiguration.DestinationBankAccountUniqueId,
                RecipientName = this.destinationBankAccountConfiguration.RecipientName
            };

            // process card
            var success = await CardPaymentsHelper.ProcessPaymentAsync(submitModel,
                this.cardPaymentsConfiguration.CentralApiCardPaymentsUrl);

            // check if the payment is successful
            if (!success)
            {
                this.ModelState.AddModelError(string.Empty, PaymentUnsuccessfulError);

                model.ProductName = order.ProductName;
                model.ProductPrice = order.ProductPrice;

                return this.View(model);
            }

            // mark the payment as completed
            await this.ordersService.SetPaymentStatus(id, PaymentStatus.Completed);

            return this.RedirectToAction("My", "Orders");
        }
    }
}