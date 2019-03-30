namespace DemoShop.Services.Implementations
{
    using System.Collections.Generic;
    using System.Linq;
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

        public async Task<OrderDetailsServiceModel> GetByIdAsync(string id)
        {
            var order = await this.Context.Orders
                .Select(o => new OrderDetailsServiceModel
                {
                    Id = o.Id,
                    CreatedOn = o.CreatedOn,
                    ProductName = o.Product.Name,
                    ProductImageUrl = o.Product.ImageUrl,
                    UserName = o.User.UserName,
                    ProductPrice = o.Product.Price,
                    PaymentStatus = o.PaymentStatus
                })
                .SingleOrDefaultAsync(p => p.Id == id);

            return order;
        }

        public async Task<IEnumerable<OrderDetailsServiceModel>> GetAllForUserAsync(string userName)
        {
            var orders = await this.Context.Orders
                .Where(p => p.User.UserName == userName)
                .OrderByDescending(p => p.CreatedOn)
                .Select(o => new OrderDetailsServiceModel
                {
                    Id = o.Id,
                    CreatedOn = o.CreatedOn,
                    ProductName = o.Product.Name,
                    ProductImageUrl = o.Product.ImageUrl,
                    UserName = o.User.UserName,
                    ProductPrice = o.Product.Price,
                    PaymentStatus = o.PaymentStatus
                })
                .ToArrayAsync();

            return orders;
        }
    }
}