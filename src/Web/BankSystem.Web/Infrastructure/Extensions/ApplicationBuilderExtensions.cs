namespace BankSystem.Web.Infrastructure.Extensions
{
    using Data;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using Middleware;

    public static class ApplicationBuilderExtensions
    {
        public static void InitializeDatabase(this IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var dbContext = serviceScope.ServiceProvider.GetService<BankSystemDbContext>();
                dbContext.Database.Migrate();
            }
        }

        public static IApplicationBuilder AddDefaultSecurityHeaders(this IApplicationBuilder app, SecurityHeadersBuilder builder)
            => app.UseMiddleware<SecurityHeadersMiddleware>(builder.Policy());
    }
}
