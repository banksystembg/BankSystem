namespace DemoShop.Services.Interfaces
{
    using System.Threading.Tasks;
    using Models;

    public interface IProductsService
    {
        Task CreateAsync(ProductCreateServiceModel model);
    }
}