namespace DemoShop.Services.Interfaces
{
    using System.Threading.Tasks;
    using Models.Order;

    public interface IOrdersService
    {
        Task<string> CreateAsync(OrderCreateServiceModel model);
    }
}