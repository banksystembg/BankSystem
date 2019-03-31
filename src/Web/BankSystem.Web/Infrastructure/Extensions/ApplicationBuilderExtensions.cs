namespace BankSystem.Web.Infrastructure.Extensions
{
    using System.Threading.Tasks;
    using Data;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using Middleware;

    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder InitializeDatabase(this IApplicationBuilder app)
        {
            InitializeDatabaseAsync(app).GetAwaiter().GetResult();

            return app;
        }

        // Migrate database and create administrator role
        private static async Task InitializeDatabaseAsync(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                var dbContext = serviceScope.ServiceProvider.GetRequiredService<BankSystemDbContext>();

                await dbContext.Database.MigrateAsync();
            }
        }

        public static IApplicationBuilder AddDefaultSecurityHeaders(this IApplicationBuilder app, SecurityHeadersBuilder builder)
            => app.UseMiddleware<SecurityHeadersMiddleware>(builder.Policy());
    }
}
