namespace CentralApi.Infrastructure
{
    using System.Linq;
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
                        ApiKey = "<RSAKeyValue><Modulus>uBJGDt7UVg068eAXtaJ8wxbTLtxJWubChoTCCljt4t8eCcUQTjbVjmiX4n9q01PyfP3Xe2MqKx0HhSygSQr4GTsPhUo44EEJr9E6ZgAjQcBJTbRop4i06BFk+u0x0P/9nZtjQQqFhKrTdVPP9rSc8CYYDsEVzsQTtjKZdtUkPFITfz6fwJHQm2Zswqwu9sSlIIwknz0jKG7lnbEwxrqQy57jjxM7ZujYCop+N20KvBIHLquuH3wkTIxmj4nZLscYBbAE7J4JhuHVE/fCJFIA+M9JgKVvEHeC5HIqshMSaAGRv9Th6HHk0v4QFm3NdpZsVf2Qhu0iOap3fHHoAypFZQ==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>",
                        ApiAddress = "https://localhost:56013/api/ReceiveMoneyTransfers",
                        PaymentUrl = "https://localhost:56013/pay/{0}"

                    },
                    new Bank
                    {
                        Location = "Finland",
                        Name = "Bank system 2",
                        ShortName = "BS2",
                        SwiftCode = "ABC2",
                        ApiKey = "<RSAKeyValue><Modulus>yBVl/2QkLXbay+SQuOwZka4zwvK+evwf7Vxwl7UIYwdpYB9gn097++mehXLcGGUEhmLp0dn6/WhRkkOuY99BbTbMNzf3+Z+VZ52HzsMqJvxn27WyN3ez98kWgOKoe+1qyqfkH1qS/Rdp/ITbF/fg7EkvNlI5ZYmK0ycNS+pk5l8wl3aYsBzSVCZwjP9/KcIm7ha4CzL1MPO4s2NjasAszz9Yh9ShtdICsWSklTpPog+xA6b3fQDdHcV9q7023fdkd0CyVK16eGbOFaN01umhN5stJQx/qrqEO6f+d8azvgAY1VCbt3b4zvtWoQJ7UpDNENcnMS+85LTWXIAnLkx5qQ==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>",
                        ApiAddress = "https://localhost:44335/api/ReceiveMoneyTransfers",
                    });
                dbContext.SaveChanges();
            }
        }
    }
}
