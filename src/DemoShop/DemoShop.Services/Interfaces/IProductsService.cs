namespace DemoShop.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Models.Product;

    public interface IProductsService
    {
        Task CreateAsync(ProductCreateServiceModel model);
        Task<IEnumerable<ProductDetailsServiceModel>> GetAllAsync();
    }
}