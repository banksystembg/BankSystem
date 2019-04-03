namespace CentralApi.Services.Tests.Tests
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

        protected CentralApiDbContext DatabaseInstance
        {
            get
            {
                var options = new DbContextOptionsBuilder<CentralApiDbContext>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString())
                    .EnableSensitiveDataLogging()
                    .Options;

                return new CentralApiDbContext(options);
            }
        }
    }
}
