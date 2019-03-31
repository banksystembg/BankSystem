namespace BankSystem.Services.Tests.Tests
{
    using BankSystem.Models;
    using Common;
    using Data;
    using FluentAssertions;
    using Implementations;
    using Interfaces;
    using Microsoft.EntityFrameworkCore;
    using Models.GlobalTransfer;
    using Moq;
    using System.Threading.Tasks;
    using Xunit;

    public class GlobalTransferHelperTests : BaseTest
    {
        private const string SampleUserId = "adfssfdgh523-sdfvxchjg0-xghholf";
        private const string SampleBankAccountName = "Test bank account name";
        private const string SampleBankAccountId = "h140gsdf-jf-gj34-hdfj-dfhdf";
        private const string SampleBankAccountUniqueId = "UniqueId";

        private const decimal SampleBalance = 1000;
        private const decimal SampleAmount = 10;
        private const string SampleRecipientName = "test";
        private const string SampleDescription = "Enjoy the money!";
        private const string SampleDestinationBankSwiftCode = "AGA";
        private const string SampleDestinationBankName = "destination bank name";
        private const string SampleDestinationBankCountry = "Switzerland";
        private const string SampleDestinationBankAccountUniqueId = "AGAE82853950";

        private readonly BankSystemDbContext dbContext;
        private readonly IGlobalTransferHelper globalTransferHelper;

        public GlobalTransferHelperTests()
        {
            this.dbContext = base.DatabaseInstance;
            this.globalTransferHelper = new GlobalTransferHelper(Mock.Of<IBankAccountService>(), Mock.Of<IMoneyTransferService>(),
                new BankConfigurationHelper(base.MockedBankConfiguration.Object));
        }

        [Fact]
        public async Task TransferMoneyAsync_WithInvalidModel_ShouldReturnGeneralFailure()
        {
            // Act
            var result = await this.globalTransferHelper.TransferMoneyAsync(new GlobalTransferServiceModel());

            // Assert
            result
                .Should()
                .Be(GlobalTransferResult.GeneralFailure);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task TransferMoneyAsync_WithInvalidAmount_ShouldReturnGeneralFailure(decimal amount)
        {
            // Arrange
            var model = PrepareTransferModel(amount: amount);
            // Act
            var result = await this.globalTransferHelper.TransferMoneyAsync(model);

            // Assert
            result
                .Should()
                .Be(GlobalTransferResult.GeneralFailure);
        }

        [Fact]
        public async Task TransferMoneyAsync_WithInvalidRecipientName_ShouldReturnGeneralFailure()
        {
            // Arrange
            var invalidName = new string('m', ModelConstants.User.FullNameMaxLength + 1);
            var model = PrepareTransferModel(recipientName: invalidName);
            // Act
            var result = await this.globalTransferHelper.TransferMoneyAsync(model);

            // Assert
            result
                .Should()
                .Be(GlobalTransferResult.GeneralFailure);
        }

        [Fact]
        public async Task TransferMoneyAsync_WithInvalidDescription_ShouldReturnGeneralFailure()
        {
            // Arrange
            var invalidDescription = new string('m', ModelConstants.MoneyTransfer.DescriptionMaxLength + 1);
            var model = PrepareTransferModel(description: invalidDescription);
            // Act
            var result = await this.globalTransferHelper.TransferMoneyAsync(model);

            // Assert
            result
                .Should()
                .Be(GlobalTransferResult.GeneralFailure);
        }

        [Fact]
        public async Task TransferMoneyAsync_WithInvalidDestinationBankSwiftCode_ShouldReturnGeneralFailure()
        {
            // Arrange
            var invalidSwiftCode = new string('m', ModelConstants.BankAccount.SwiftCodeMaxLength + 1);
            var model = PrepareTransferModel(destinationBankSwiftCode: invalidSwiftCode);
            // Act
            var result = await this.globalTransferHelper.TransferMoneyAsync(model);

            // Assert
            result
                .Should()
                .Be(GlobalTransferResult.GeneralFailure);
        }

        [Fact]
        public async Task TransferMoneyAsync_WithInvalidDestinationBankName_ShouldReturnGeneralFailure()
        {
            // Arrange
            var invalidName = new string('m', ModelConstants.BankAccount.NameMaxLength + 1);
            var model = PrepareTransferModel(destinationBankName: invalidName);
            // Act
            var result = await this.globalTransferHelper.TransferMoneyAsync(model);

            // Assert
            result
                .Should()
                .Be(GlobalTransferResult.GeneralFailure);
        }

        [Fact]
        public async Task TransferMoneyAsync_WithInvalidDestinationBankCountry_ShouldReturnGeneralFailure()
        {
            // Arrange
            var invalidCountry = new string('m', ModelConstants.BankAccount.CountryMaxLength + 1);
            var model = PrepareTransferModel(destinationBankCountry: invalidCountry);
            // Act
            var result = await this.globalTransferHelper.TransferMoneyAsync(model);

            // Assert
            result
                .Should()
                .Be(GlobalTransferResult.GeneralFailure);
        }

        [Fact]
        public async Task TransferMoneyAsync_WithInvalidDestinationBankAccountUniqueId_ShouldReturnGeneralFailure()
        {
            // Arrange
            var invalidAccountUniqueId = new string('m', ModelConstants.BankAccount.UniqueIdMaxLength + 1);
            var model = PrepareTransferModel(destinationBankAccountUniqueId: invalidAccountUniqueId);
            // Act
            var result = await this.globalTransferHelper.TransferMoneyAsync(model);

            // Assert
            result
                .Should()
                .Be(GlobalTransferResult.GeneralFailure);
        }

        #region privateMethods

        private static GlobalTransferServiceModel PrepareTransferModel(
            string sourceAccountId = SampleBankAccountId,
            decimal amount = SampleAmount,
            string recipientName = SampleRecipientName,
            string description = SampleDescription,
            string destinationBankSwiftCode = SampleDestinationBankSwiftCode,
            string destinationBankName = SampleDestinationBankName,
            string destinationBankCountry = SampleDestinationBankCountry,
            string destinationBankAccountUniqueId = SampleDestinationBankAccountUniqueId)
        {
            var model = new GlobalTransferServiceModel
            {
                SourceAccountId = sourceAccountId,
                Amount = amount,
                RecipientName = recipientName,
                Description = description,
                DestinationBankSwiftCode = destinationBankSwiftCode,
                DestinationBankName = destinationBankName,
                DestinationBankCountry = destinationBankCountry,
                DestinationBankAccountUniqueId = destinationBankAccountUniqueId
            };

            return model;
        }
        private async Task<BankAccount> SeedBankAccountAsync()
        {
            await this.SeedUserAsync();
            var model = new BankAccount
            {
                Id = SampleBankAccountId,
                Name = SampleBankAccountName,
                UniqueId = SampleBankAccountUniqueId,
                User = await this.dbContext.Users.FirstOrDefaultAsync(),
                Balance = SampleBalance,

            };
            await this.dbContext.Accounts.AddAsync(model);
            await this.dbContext.SaveChangesAsync();

            return model;
        }

        private async Task SeedUserAsync()
        {
            await this.dbContext.Users.AddAsync(new BankUser { Id = SampleUserId, });
            await this.dbContext.SaveChangesAsync();
        }

        #endregion
    }
}
