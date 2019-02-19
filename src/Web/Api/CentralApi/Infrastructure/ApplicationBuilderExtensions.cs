namespace CentralApi.Infrastructure
{
    using System;
    using System.Linq;
    using System.Security.Cryptography;
    using Data;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using Models;

    public static class ApplicationBuilderExtensions
    {
        public static void SeedData(this IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var dbContext = serviceScope.ServiceProvider.GetService<CentralApiDbContext>();
                dbContext.Database.Migrate();

                // Seed data
                SeedAvailableBanks(dbContext);
            }
        }

        private static void SeedAvailableBanks(CentralApiDbContext dbContext)
        {
            if (!dbContext.Banks.Any())
            {
                dbContext.AddRange(
                    new Bank
                    {
                        Location = "Bulgaria",
                        Name = "Bank system",
                        ShortName = "BS",
                        SwiftCode = "ABC",
                        ApiKey = GenerateApiKey(),
                        AppId = Guid.NewGuid().ToString(),

                    },
                    new Bank
                    {
                        Location = "Bulgaria",
                        Name = "Bank System 2",
                        ShortName = "BS2",
                        SwiftCode = "ABC2",
                        ApiKey = GenerateApiKey(),
                        AppId = Guid.NewGuid().ToString(),
                    });
                dbContext.SaveChanges();
            }
        }

        private static string GenerateApiKey()
        {
            using (var cryptoProvider = new RNGCryptoServiceProvider())
            {
                //256 bit
                var secretKeyByteArray = new byte[32];
                cryptoProvider.GetBytes(secretKeyByteArray);
                return Convert.ToBase64String(secretKeyByteArray);
            }
        }
    }
}
