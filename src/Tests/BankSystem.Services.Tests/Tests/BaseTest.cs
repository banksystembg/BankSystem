namespace BankSystem.Services.Tests.Tests
{
    using Data;
    using Microsoft.EntityFrameworkCore;
    using Setup;
    using System;

    public abstract class BaseTest
    {
        protected BaseTest()
        {
            TestSetup.InitializeMapper();
        }

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
