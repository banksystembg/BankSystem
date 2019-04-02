namespace CentralApi.Data
{
    using Microsoft.EntityFrameworkCore;
    using Models;

    public class CentralApiDbContext : DbContext
    {
        public CentralApiDbContext(DbContextOptions<CentralApiDbContext> options)
            : base(options)
        {
        }

        public DbSet<Bank> Banks { get; set; }
    }
}