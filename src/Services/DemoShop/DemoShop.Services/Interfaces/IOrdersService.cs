namespace DemoShop.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using DemoShop.Models;
    using Models.Order;

    public interface IOrdersService
    {
        Task<string> CreateAsync(OrderCreateServiceModel model);
        Task<OrderDetailsServiceModel> GetByIdAsync(string id);
        Task<IEnumerable<OrderDetailsServiceModel>> GetAllForUserAsync(string userName);
        Task SetPaymentStatus(string orderId, PaymentStatus paymentStatus);
    }
}