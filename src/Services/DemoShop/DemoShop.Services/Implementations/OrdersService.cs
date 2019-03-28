namespace DemoShop.Services.Implementations
{
    using System.Threading.Tasks;
    using Data;
    using DemoShop.Models;
    using Interfaces;
    using Microsoft.EntityFrameworkCore;
    using Models.Order;

    public class OrdersService : BaseService, IOrdersService
    {
        public OrdersService(DemoShopDbContext context) : base(context)
        {
        }

        public async Task<string> CreateAsync(OrderCreateServiceModel model)
        {
            if (!IsEntityStateValid(model))
            {
                return null;
            }

            var user = await this.Context.Users.SingleOrDefaultAsync(u => u.UserName == model.UserName);

            if (user == null ||
                !await this.Context.Products.AnyAsync(b => b.Id == model.ProductId))
            {
                return null;
            }

            var order = new Order
            {
                ProductId = model.ProductId,
                UserId = user.Id,
                CreatedOn = model.CreatedOn,
                PaymentStatus = PaymentStatus.Pending
            };

            await this.Context.Orders.AddAsync(order);

            await this.Context.SaveChangesAsync();

            return order.Id;
        }
    }
}