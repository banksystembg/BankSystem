namespace DemoShop.Web.Controllers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Services.Interfaces;
    using Services.Models.Order;

    [Authorize]
    public class OrdersController : Controller
    {
        public const string CardPaymentMethodName = "Card";
        public const string DirectPaymentMethodName = "Direct";

        private readonly IOrdersService ordersService;

        public OrdersController(IOrdersService ordersService)
            => this.ordersService = ordersService;

        public async Task<IActionResult> My()
        {
            var orders = (await this.ordersService.GetAllForUserAsync(this.User.Identity.Name))
                .Select(o => new OrderDetailsViewModel
                {
                    Id = o.Id,
                    CreatedOn = o.CreatedOn,
                    ProductName = o.ProductName,
                    ProductImageUrl = o.ProductImageUrl,
                    ProductPrice = o.ProductPrice,
                    PaymentStatus = o.PaymentStatus
                })
                .ToArray();

            return this.View(orders);
        }

        [HttpPost]
        public async Task<IActionResult> OrderProduct(string productId, string paymentMethod)
        {
            if (productId == null)
            {
                return this.RedirectToAction("Index", "Home");
            }

            var serviceModel = new OrderCreateServiceModel
            {
                ProductId = productId,
                CreatedOn = DateTime.UtcNow,
                UserName = this.User.Identity.Name
            };

            string orderId = await this.ordersService.CreateAsync(serviceModel);

            if (orderId == null)
            {
                return this.RedirectToAction("Index", "Home");
            }

            if (paymentMethod == DirectPaymentMethodName)
            {
                return this.RedirectToAction("Pay", "DirectPayments", new {id = orderId});
            }

            if (paymentMethod == CardPaymentMethodName)
            {
                return this.RedirectToAction("Pay", "CardPayments", new {id = orderId});
            }

            return this.RedirectToAction("My");
        }
    }
}