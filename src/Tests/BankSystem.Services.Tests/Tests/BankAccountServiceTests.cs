namespace BankSystem.Services.Tests.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using BankAccount;
    using BankSystem.Models;
    using Data;
    using FluentAssertions;
    using Microsoft.EntityFrameworkCore;
    using Models.BankAccount;
    using Xunit;

    public class BankAccountServiceTests : BaseTest
    {
        public BankAccountServiceTests()
        {
            this.dbContext = this.DatabaseInstance;
            this.bankAccountService = new BankAccountService(this.dbContext,
                new BankAccountUniqueIdHelper(this.MockedBankConfiguration.Object), this.Mapper);
        }

        private const string SampleBankAccountName = "Test bank account name";
        private const string SampleBankAccountUserId = "adfsdvxc-123ewsf";
        private const string SampleBankAccountId = "1";
        private const string SampleBankAccountUniqueId = "UniqueId";

        private readonly BankSystemDbContext dbContext;
        private readonly IBankAccountService bankAccountService;

        [Theory]
        [InlineData("-1")]
        [InlineData("   ")]
        [InlineData("")]
        [InlineData("someRandomValue")]
        public async Task ChangeAccountNameAsync_WithInvalidId_Should_ReturnFalse(string id)
        {
            // Arrange
            await this.SeedBankAccountAsync();

            // Act
            var result = await this.bankAccountService.ChangeAccountNameAsync(id, SampleBankAccountName);

            // Assert
            result
                .Should()
                .BeFalse();
        }

        private async Task SeedUserAsync()
        {
            await this.dbContext.Users.AddAsync(new BankUser
                { Id = SampleBankAccountUserId, FullName = SampleBankAccountUniqueId });
            await this.dbContext.SaveChangesAsync();
        }

        private async Task<BankAccount> SeedBankAccountAsync()
        {
            var model = new BankAccount
            {
                Id = SampleBankAccountId,
                Name = SampleBankAccountName,
                UniqueId = SampleBankAccountUniqueId,
                UserId = SampleBankAccountUserId
            };
            await this.dbContext.Accounts.AddAsync(model);
            await this.dbContext.SaveChangesAsync();

            return model;
        }

        [Fact]
        public async Task ChangeAccountNameAsync_WithValidId_Should_ReturnTrue_And_ChangeNameSuccessfully()
        {
            // Arrange
            var model = await this.SeedBankAccountAsync();
            var newName = "changed!";

            // Act
            var result = await this.bankAccountService.ChangeAccountNameAsync(model.Id, newName);

            // Assert
            result
                .Should()
                .BeTrue();

            // Ensure that name is changed
            var dbModel = await this.dbContext
                .Accounts
                .FindAsync(model.Id);

            dbModel.Name
                .Should()
                .BeEquivalentTo(newName);
        }

        [Fact]
        public async Task CreateAsync_WithInvalidNameLength_Should_ReturnNull_And_NotInsertInDatabase()
        {
            // Arrange
            await this.SeedUserAsync();
            // Name is invalid when it's longer than 35 characters
            var model = new BankAccountCreateServiceModel
                { Name = new string('c', 36), UserId = SampleBankAccountUserId };

            // Act
            var result = await this.bankAccountService.CreateAsync(model);

            // Assert
            result
                .Should()
                .BeNull();

            this.dbContext
                .Accounts
                .Should()
                .BeEmpty();
        }

        [Fact]
        public async Task CreateAsync_WithInvalidUserId_Should_ReturnNull_And_NotInsertInDatabase()
        {
            // Arrange
            await this.SeedUserAsync();
            var model = new BankAccountCreateServiceModel { Name = SampleBankAccountName, UserId = null };

            // Act
            var result = await this.bankAccountService.CreateAsync(model);

            // Assert
            result
                .Should()
                .BeNull();

            this.dbContext
                .Accounts
                .Should()
                .BeEmpty();
        }

        [Fact]
        public async Task CreateAsync_WithValidModel_AndEmptyName_Should_SetRandomString()
        {
            // Arrange
            var count = this.dbContext.Accounts.Count();
            await this.SeedUserAsync();
            var model = new BankAccountCreateServiceModel { UserId = SampleBankAccountUserId };

            // Act
            var result = await this.bankAccountService.CreateAsync(model);

            // Assert
            result
                .Should()
                .NotBeNullOrEmpty()
                .And
                .BeAssignableTo<string>();

            this.dbContext
                .Accounts
                .Should()
                .HaveCount(count + 1);
        }

        [Fact]
        public async Task CreateAsync_WithValidModel_Should__InsertInDatabase()
        {
            // Arrange
            var count = this.dbContext.Accounts.Count();
            await this.SeedUserAsync();
            var model = new BankAccountCreateServiceModel { UserId = SampleBankAccountUserId };

            // Act
            await this.bankAccountService.CreateAsync(model);

            this.dbContext
                .Accounts
                .Should()
                .HaveCount(count + 1);
        }

        [Fact]
        public async Task CreateAsync_WithValidModel_Should_ReturnNonEmptyString()
        {
            // Arrange
            await this.SeedUserAsync();
            // CreatedOn is not required since it has default value which is set from the class - Datetime.UtcNow
            var model = new BankAccountCreateServiceModel
                { Name = SampleBankAccountName, UserId = SampleBankAccountUserId, CreatedOn = DateTime.UtcNow };

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
        public async Task GetAccountsAsync_Should_ReturnCollectionWithCorrectModels()
        {
            // Arrange
            await this.SeedBankAccountAsync();

            // Act
            var result = await this.bankAccountService.GetAccountsAsync<BankAccountDetailsServiceModel>();

            // Assert
            result
                .Should()
                .AllBeAssignableTo<IEnumerable<BankAccountDetailsServiceModel>>();
        }

        [Fact]
        public async Task GetAllAccountsByUserIdAsync_WithInvalidId_Should_ReturnEmptyModel()
        {
            // Arrange
            await this.SeedBankAccountAsync();
            // Act
            var result =
                await this.bankAccountService.GetAllAccountsByUserIdAsync<BankAccountIndexServiceModel>(null);

            // Assert
            result
                .Should()
                .BeNullOrEmpty();
        }

        [Fact]
        public async Task GetAllAccountsByUserIdAsync_WithValidId_Should_ReturnCorrectModel()
        {
            // Arrange
            var model = await this.SeedBankAccountAsync();
            // Act
            var result =
                await this.bankAccountService.GetAllAccountsByUserIdAsync<BankAccountIndexServiceModel>(model.UserId);

            // Assert
            result
                .Should()
                .BeAssignableTo<IEnumerable<BankAccountIndexServiceModel>>();
        }

        [Fact]
        public async Task GetCountOfAccountsAsync_Should_ReturnCorrectCount()
        {
            // Arrange
            await this.SeedBankAccountAsync();

            // Act
            var result =
                await this.bankAccountService.GetCountOfAccountsAsync();

            // Assert
            result
                .Should()
                .Be(await this.dbContext.Accounts.CountAsync());
        }

        [Fact]
        public async Task GetByIdAsync_WithInvalidBankAccountId_Should_ReturnNull()
        {
            // Arrange
            await this.SeedBankAccountAsync();

            // Act
            var result = await this.bankAccountService.GetByIdAsync<BankAccountConciseServiceModel>(null);

            // Arrange
            result
                .Should()
                .BeNull();
        }

        [Fact]
        public async Task GetByIdAsync_WithValidBankAccountId_Should_ReturnCorrectEntity()
        {
            // Arrange
            var model = await this.SeedBankAccountAsync();
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
        public async Task GetByUniqueIdAsync_WithInvalidUniqueId_Should_ReturnNull()
        {
            // Arrange
            await this.SeedBankAccountAsync();

            // Act
            var result = await this.bankAccountService.GetByUniqueIdAsync<BankAccountIndexServiceModel>(null);

            // Arrange
            result
                .Should()
                .BeNull();
        }

        [Fact]
        public async Task GetByUniqueIdAsync_WithValidUniqueId_Should_ReturnCorrectEntity()
        {
            // Arrange
            var model = await this.SeedBankAccountAsync();
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
    }
}