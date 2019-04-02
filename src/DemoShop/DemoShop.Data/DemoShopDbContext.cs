namespace DemoShop.Data
{
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    using Models;

    public class DemoShopDbContext : IdentityDbContext<DemoShopUser>
    {
        public DemoShopDbContext(DbContextOptions<DemoShopDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }

        public DbSet<Order> Orders { get; set; }
    }
}