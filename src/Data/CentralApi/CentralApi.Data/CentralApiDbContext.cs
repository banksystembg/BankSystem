namespace CentralApi.Data
{
    using CentralApi.Models;
    using Microsoft.EntityFrameworkCore;

    public class CentralApiDbContext : DbContext
    {
        public CentralApiDbContext(DbContextOptions<CentralApiDbContext> options)
            : base(options)
        {
        }

        public DbSet<Bank> Banks { get; set; }
    }
}
