namespace DemoShop.Web.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Services.Interfaces;
    using Services.Models;
    using Services.Models.Product;

    [Authorize]
    public class ProductsController : Controller
    {
        private readonly IProductsService productsService;

        public ProductsController(IProductsService productsService)
        {
            this.productsService = productsService;
        }

        public IActionResult Create()
        {
            return this.View();
        }

        [HttpPost]
        public IActionResult Create(ProductCreateBindingModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            var serviceProduct = new ProductCreateServiceModel
            {
                Name = model.Name,
                Price = model.Price,
                ImageUrl = model.ImageUrl
            };

            this.productsService.CreateAsync(serviceProduct);

            return this.RedirectToAction("Index", "Home");
        }
    }
}