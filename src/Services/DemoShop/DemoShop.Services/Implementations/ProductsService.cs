namespace DemoShop.Services.Implementations
{
    using System.Threading.Tasks;
    using Data;
    using DemoShop.Models;
    using Interfaces;
    using Models;

    public class ProductsService : BaseService, IProductsService
    {
        public ProductsService(DemoShopDbContext context) : base(context)
        {
        }

        public async Task CreateAsync(ProductCreateServiceModel model)
        {
            if (!IsEntityStateValid(model))
            {
                return;
            }

            var dbModel = new Product
            {
                Name = model.Name,
                Price = model.Price,
                ImageUrl = model.ImageUrl
            };

            await this.Context.Products.AddAsync(dbModel);

            await this.Context.SaveChangesAsync();
        }
    }
}