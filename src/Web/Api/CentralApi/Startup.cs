namespace CentralApi
{
    using System;
    using AutoMapper;
    using BankSystem.Common.AutoMapping.Profiles;
    using BankSystem.Common.Utils;
    using Data;
    using Infrastructure;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Services.Bank;

    public class Startup
    {
        public Startup(IConfiguration configuration)
            => this.Configuration = configuration;

        private IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<CentralApiDbContext>(options =>
                options.UseSqlServer(
                    this.Configuration.GetConnectionString("DefaultConnection")));
            services.AddScoped<IBanksService, BanksService>();

            services.Configure<CentralApiConfiguration>(
                this.Configuration.GetSection(nameof(CentralApiConfiguration)));

            services.PostConfigure<CentralApiConfiguration>(settings =>
            {
                if (!ValidationUtil.IsObjectValid(settings))
                {
                    throw new ApplicationException("CentralApiConfiguration is invalid");
                }
            });
            services.AddAutoMapper(typeof(DefaultProfile));
            services
                .AddControllersWithViews();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

            app.SeedData();
        }
    }
}