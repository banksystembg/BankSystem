namespace BankSystem.Services.Tests.Tests
{
    using System;
    using AutoMapper;
    using Common.Configuration;
    using Data;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;
    using Moq;
    using Setup;

    public abstract class BaseTest
    {
        private const string SampleBankName = "Bank system";
        private const string SampleUniqueIdentifier = "ABC";
        private const string SampleFirst3CardDigits = "123";
        private const string SampleCentralApiAddress = "https://localhost:5001/";
        private const string SampleCentralApiPublicKey = "sdgijsd09gusd0jsdpfasjiofasd";
        private const string SampleBankCountry = "Bulgaria";
        private const string SampleBankKey = "sdf90234rewfsd0ij9oigsdf";

        protected BaseTest()
            => this.Mapper = TestSetup.InitializeMapper();

        protected IMapper Mapper { get; }

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

        protected Mock<IOptions<BankConfiguration>> MockedBankConfiguration
        {
            get
            {
                var bankConfiguration = new BankConfiguration
                {
                    BankName = SampleBankName,
                    UniqueIdentifier = SampleUniqueIdentifier,
                    First3CardDigits = SampleFirst3CardDigits,
                    CentralApiAddress = SampleCentralApiAddress,
                    CentralApiPublicKey = SampleCentralApiPublicKey,
                    Country = SampleBankCountry,
                    Key = SampleBankKey
                };

                var options = new Mock<IOptions<BankConfiguration>>();
                options.Setup(x => x.Value).Returns(bankConfiguration);

                return options;
            }
        }
    }
}