namespace BankSystem.Services.Tests.Tests
{
    using BankSystem.Models;
    using Common.Configuration;
    using Data;
    using FluentAssertions;
    using Implementations;
    using Interfaces;
    using Microsoft.Extensions.Options;
    using Models.BankAccount;
    using Moq;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Xunit;

    public class BankAccountServiceTests : BaseTest
    {
        private const string SampleBankAccountName = "Test bank account name";
        private const string SampleBankAccountUserId = "adfsdvxc-123ewsf";
        private const string SampleBankAccountId = "1";
        private const string SampleBankAccountUniqueId = "UniqueId";
        private const string SampleBankName = "Bank system";
        private const string SampleUniqueIdentifier = "ABC";
        private const string SampleFirst3CardDigits = "123";

        private readonly BankSystemDbContext dbContext;
        private readonly IBankAccountService bankAccountService;

        public BankAccountServiceTests()
        {

            this.dbContext = base.DatabaseInstance;
            var bankConfiguration = new BankConfiguration
            {
                BankName = SampleBankName,
                UniqueIdentifier = SampleUniqueIdentifier,
                First3CardDigits = SampleFirst3CardDigits,
            };
            var options = new Mock<IOptions<BankConfiguration>>();
            options.Setup(x => x.Value).Returns(bankConfiguration);
            this.bankAccountService = new BankAccountService(this.dbContext, new BankAccountUniqueIdHelper(new BankConfigurationHelper(options.Object)));
        }

        [Fact]
        public async Task CreateAsync_WithInvalidUserId_ShouldReturnNull()
        {
            // Arrange
            await this.SeedUser();
            var model = new BankAccountCreateServiceModel { Name = SampleBankAccountName, UserId = null };

            // Act
            var result = await this.bankAccountService.CreateAsync(model);

            // Assert
            result
                .Should()
                .BeNull();
        }

        [Fact]
        public async Task CreateAsync_WithInvalidNameLength_ShouldReturnNull()
        {
            // Arrange
            await this.SeedUser();
            // Name is invalid when it's longer than 35 characters
            var model = new BankAccountCreateServiceModel { Name = new string('c', 36), UserId = SampleBankAccountUserId };

            // Act
            var result = await this.bankAccountService.CreateAsync(model);

            // Assert
            result
                .Should()
                .BeNull();
        }

        [Fact]
        public async Task CreateAsync_WithValidModel_AndEmptyName_ShouldSetRandomString_And_ShouldReturnNonEmptyString()
        {
            // Arrange
            await this.SeedUser();
            var model = new BankAccountCreateServiceModel { UserId = SampleBankAccountUserId };

            // Act
            var result = await this.bankAccountService.CreateAsync(model);

            // Assert
            result
                .Should()
                .NotBeNullOrEmpty()
                .And
                .BeAssignableTo<string>();
        }

        [Fact]
        public async Task CreateAsync_WithValidModel_ShouldReturnNonEmptyString()
        {
            // Arrange
            await this.SeedUser();
            // CreatedOn is not required since it has default value which is set from the class - Datetime.UtcNow
            var model = new BankAccountCreateServiceModel { Name = SampleBankAccountName, UserId = SampleBankAccountUserId, CreatedOn = DateTime.UtcNow };

            // Act
            var result = await this.bankAccountService.CreateAsync(model);

            // Assert
            result
                .Should()
                .NotBeNullOrEmpty()
                .And
                .BeAssignableTo<string>();
        }

        [Fact]
        public async Task GetByUniqueIdAsync_WithInvalidUniqueId_ShouldReturnNull()
        {
            // Arrange
            await this.SeedBankAccount();

            // Act
            var result = await this.bankAccountService.GetByUniqueIdAsync<BankAccountIndexServiceModel>(null);

            // Arrange
            result
                .Should()
                .BeNull();
        }

        [Fact]
        public async Task GetByUniqueIdAsync_WithValidUniqueId_ShouldReturnCorrectEntity()
        {
            // Arrange
            var model = await this.SeedBankAccount();
            var expectedUniqueId = model.UniqueId;

            // Act
            var result = await this.bankAccountService.GetByUniqueIdAsync<BankAccountIndexServiceModel>(model.UniqueId);

            // Arrange
            result
                .Should()
                .NotBeNull()
                .And
                .Match(x => x.As<BankAccountIndexServiceModel>().UniqueId == expectedUniqueId);
        }

        [Fact]
        public async Task GetByIdAsync_WithInvalidBankAccountId_ShouldReturnNull()
        {
            // Arrange
            await this.SeedBankAccount();

            // Act
            var result = await this.bankAccountService.GetByIdAsync<BankAccountConciseServiceModel>(null);

            // Arrange
            result
                .Should()
                .BeNull();
        }

        [Fact]
        public async Task GetByIdAsync_WithValidBankAccountId_ShouldReturnCorrectEntity()
        {
            // Arrange
            var model = await this.SeedBankAccount();
            var expectedId = model.Id;
            var expectedUniqueId = model.UniqueId;

            // Act
            var result = await this.bankAccountService.GetByIdAsync<BankAccountIndexServiceModel>(model.Id);

            // Arrange
            result
                .Should()
                .NotBeNull()
                .And
                .Match(x => x.As<BankAccountIndexServiceModel>().Id == expectedId)
                .And
                .Match(x => x.As<BankAccountIndexServiceModel>().UniqueId == expectedUniqueId);
        }

        [Fact]
        public async Task GetAllAccountsByUserIdAsync_WithInvalidId_ShouldReturnEmptyModel()
        {
            // Arrange
            await this.SeedBankAccount();
            // Act
            var result =
                await this.bankAccountService.GetAllAccountsByUserIdAsync<BankAccountIndexServiceModel>(null);

            // Assert
            result
                .Should()
                .BeNullOrEmpty();
        }

        [Fact]
        public async Task GetAllAccountsByUserIdAsync_WithValidId_ShouldReturnCorrectModel()
        {
            // Arrange
            var model = await this.SeedBankAccount();
            // Act
            var result =
                await this.bankAccountService.GetAllAccountsByUserIdAsync<BankAccountIndexServiceModel>(model.UserId);

            // Assert
            result
                .Should()
                .BeAssignableTo<IEnumerable<BankAccountIndexServiceModel>>();
        }

        #region privateMethods

        private async Task SeedUser()
        {
            await this.dbContext.Users.AddAsync(new BankUser { Id = SampleBankAccountUserId, FullName = SampleBankAccountUniqueId });
            await this.dbContext.SaveChangesAsync();
        }

        private async Task<BankAccount> SeedBankAccount()
        {
            var model = new BankAccount
            {
                Id = SampleBankAccountId,
                Name = SampleBankAccountName,
                UniqueId = SampleBankAccountUniqueId,
                UserId = SampleBankAccountUserId,

            };
            await this.dbContext.Accounts.AddAsync(model);
            await this.dbContext.SaveChangesAsync();

            return model;
        }

        #endregion
    }
}
