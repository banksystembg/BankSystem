namespace DemoShop.Web.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Services.Interfaces;

    public class HomeController : Controller
    {
        private readonly IProductsService productsService;

        public HomeController(IProductsService productsService)
            => this.productsService = productsService;

        public async Task<IActionResult> Index()
        {
            var serviceProducts = await this.productsService.GetAllAsync();

            var viewProducts = serviceProducts.Select(p => new ProductDetailsViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                ImageUrl = p.ImageUrl
            }).ToArray();

            var homeViewModel = new HomeViewModel
            {
                Products = viewProducts
            };

            return this.View(homeViewModel);
        }
    }
}