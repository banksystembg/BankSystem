namespace DemoShop.Web.Controllers
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Services.Interfaces;
    using Services.Models.Product;

    [Authorize]
    public class ProductsController : Controller
    {
        private readonly IProductsService productsService;

        public ProductsController(IProductsService productsService)
            => this.productsService = productsService;

        public IActionResult Create()
            => this.View();

        [HttpPost]
        public async Task<IActionResult> Create(ProductCreateBindingModel model)
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

            await this.productsService.CreateAsync(serviceProduct);

            return this.RedirectToAction("Index", "Home");
        }
    }
}