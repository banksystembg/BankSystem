namespace BankSystem.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Models;

    public class UserConfiguration : IEntityTypeConfiguration<BankUser>
    {
        public void Configure(EntityTypeBuilder<BankUser> builder)
        {
            builder
                .HasMany(u => u.Cards)
                .WithOne(c => c.User)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}