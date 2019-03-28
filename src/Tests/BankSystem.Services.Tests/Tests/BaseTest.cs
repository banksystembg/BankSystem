namespace BankSystem.Services.Tests.Tests
{
    using Data;
    using Microsoft.EntityFrameworkCore;
    using System;

    public abstract class BaseTest
    {
        protected BankSystemDbContext DatabaseInstance
        {
            get
            {
                var options = new DbContextOptionsBuilder<BankSystemDbContext>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString())
                    .EnableSensitiveDataLogging()
                    .Options;

                return new BankSystemDbContext(options);
            }
        }
    }
}
