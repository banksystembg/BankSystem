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

            builder.Entity<BankAccount>()
                .Property(e => e.Balance)
                .HasPrecision(18, 4);

            builder.Entity<MoneyTransfer>()
                .Property(e => e.Amount)
                .HasPrecision(18, 4);

            base.OnModelCreating(builder);
        }
    }
}