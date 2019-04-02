namespace BankSystem.Data
{
    using Configurations;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    using Models;

    public class BankSystemDbContext : IdentityDbContext<BankUser>
    {
        public BankSystemDbContext(DbContextOptions<BankSystemDbContext> options)
            : base(options)
        {
        }

        public DbSet<BankAccount> Accounts { get; set; }

        public DbSet<MoneyTransfer> Transfers { get; set; }

        public DbSet<Card> Cards { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new UserConfiguration());

            base.OnModelCreating(builder);
        }
    }
}