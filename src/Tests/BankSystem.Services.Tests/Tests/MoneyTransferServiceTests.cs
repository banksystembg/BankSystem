namespace BankSystem.Services.Tests.Tests
{
    using BankSystem.Models;
    using Common.EmailSender.Interface;
    using Data;
    using FluentAssertions;
    using Implementations;
    using Interfaces;
    using Microsoft.EntityFrameworkCore;
    using Models.MoneyTransfer;
    using Moq;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    public class MoneyTransferServiceTests : BaseTest
    {
        private const string SampleId = "gsdxcvew-sdfdscx-xcgds";
        private const string SampleDescription = "I'm sending money due to...";
        private const decimal SampleAmount = 10;
        private const string SampleRecipientName = "test";
        private const string SampleSenderName = "melik";
        private const string SampleDestination = "dgsfcx-arq12wasdasdzxxcv";

        private const string SampleBankAccountName = "Test bank account name";
        private const string SampleBankAccountUserId = "adfsdvxc-123ewsf";
        private const string SampleBankAccountId = "1";
        private const string SampleBankAccountUniqueId = "UniqueId";

        private readonly BankSystemDbContext dbContext;
        private readonly IMoneyTransferService moneyTransferService;

        public MoneyTransferServiceTests()
        {
            this.dbContext = base.DatabaseInstance;
            this.moneyTransferService = new MoneyTransferService(this.dbContext, Mock.Of<IEmailSender>());
        }

        [Theory]
        [InlineData(null)]
        [InlineData(" dassdgdf")]
        [InlineData(" ")]
        [InlineData("!  ")]
        [InlineData("     10   ")]
        [InlineData("  sdgsfcx-arq12wasdasdzxxcvc   ")]
        public async Task GetAllMoneyTransfersAsync_WithInvalidUserId_ShouldReturnEmptyCollection(string userId)
        {
            // Arrange
            await this.SeedMoneyTransfersAsync();
            // Act
            var result =
                await this.moneyTransferService.GetAllMoneyTransfersAsync<MoneyTransferListingServiceModel>(userId);

            // Assert
            result
                .Should()
                .BeNullOrEmpty();
        }

        [Fact]
        public async Task GetAllMoneyTransfersAsync_WithValidUserId_ShouldReturnCollectionOfCorrectEntities()
        {
            // Arrange
            var model = await this.SeedMoneyTransfersAsync();
            // Act
            var result =
                await this.moneyTransferService.GetAllMoneyTransfersAsync<MoneyTransferListingServiceModel>(model.Account.UserId);

            // Assert
            result
                .Should()
                .AllBeAssignableTo<MoneyTransferListingServiceModel>()
                .And
                .Match<IEnumerable<MoneyTransferListingServiceModel>>(x => x.All(c => c.Source == model.Source));
        }

        [Fact]
        public async Task GetAllMoneyTransfersAsync_ShouldReturnOrderedByMadeOnCollection()
        {
            // Arrange
            await this.SeedBankAccountAsync();
            for (int i = 0; i < 10; i++)
            {
                await this.dbContext.Transfers.AddAsync(new MoneyTransfer
                {
                    Id = $"{SampleId}_{i}",
                    Account = await this.dbContext.Accounts.FirstOrDefaultAsync(),
                    MadeOn = DateTime.UtcNow.AddMinutes(i)
                });
            }

            await this.dbContext.SaveChangesAsync();
            // Act
            var result =
                await this.moneyTransferService.GetAllMoneyTransfersAsync<MoneyTransferListingServiceModel>(SampleBankAccountUserId);

            // Assert
            result
                .Should()
                .BeInDescendingOrder(x => x.MadeOn);
        }

        [Fact]
        public async Task GetAllMoneyTransfersAsync_ShouldReturnCorrectCount()
        {
            // Arrange
            const int count = 10;
            await this.SeedBankAccountAsync();
            for (int i = 1; i <= count; i++)
            {
                await this.dbContext.Transfers.AddAsync(new MoneyTransfer
                {
                    Account = await this.dbContext.Accounts.FirstOrDefaultAsync(),
                });
            }

            await this.dbContext.SaveChangesAsync();
            // Act
            var result =
                await this.moneyTransferService.GetAllMoneyTransfersAsync<MoneyTransferListingServiceModel>(SampleBankAccountUserId);

            // Assert
            result
                .Should()
                .HaveCount(count);
        }

        [Theory]
        [InlineData(null)]
        [InlineData(" dassdgdf")]
        [InlineData(" ")]
        [InlineData("!  ")]
        [InlineData("     10   ")]
        [InlineData("  sdgsfcx-arq12wasdasdzxxcvc   ")]
        public async Task GetAllMoneyTransfersForAccountAsync_WithInvalidUserId_ShouldReturnEmptyCollection(string accountId)
        {
            // Arrange
            await this.SeedMoneyTransfersAsync();
            // Act
            var result =
                await this.moneyTransferService.GetAllMoneyTransfersForAccountAsync<MoneyTransferListingServiceModel>(accountId);

            // Assert
            result
                .Should()
                .BeNullOrEmpty();
        }

        [Fact]
        public async Task GetAllMoneyTransfersForAccountAsync_WithValidUserId_ShouldReturnCollectionOfCorrectEntities()
        {
            // Arrange
            var model = await this.SeedMoneyTransfersAsync();
            // Act
            var result =
                await this.moneyTransferService.GetAllMoneyTransfersForAccountAsync<MoneyTransferListingServiceModel>(model.Account.Id);

            // Assert
            result
                .Should()
                .AllBeAssignableTo<MoneyTransferListingServiceModel>()
                .And
                .Match<IEnumerable<MoneyTransferListingServiceModel>>(x => x.All(c => c.Source == model.Source));
        }

        [Fact]
        public async Task GetAllMoneyTransfersForAccountAsync_ShouldReturnOrderedByMadeOnCollection()
        {
            // Arrange
            await this.SeedBankAccountAsync();
            for (int i = 0; i < 10; i++)
            {
                await this.dbContext.Transfers.AddAsync(new MoneyTransfer
                {
                    Id = $"{SampleId}_{i}",
                    Account = await this.dbContext.Accounts.FirstOrDefaultAsync(),
                    MadeOn = DateTime.UtcNow.AddMinutes(i)
                });
            }

            await this.dbContext.SaveChangesAsync();
            // Act
            var result =
                await this.moneyTransferService.GetAllMoneyTransfersForAccountAsync<MoneyTransferListingServiceModel>(SampleBankAccountId);

            // Assert
            result
                .Should()
                .BeInDescendingOrder(x => x.MadeOn);
        }

        [Fact]
        public async Task GetAllMoneyTransfersForAccountAsync_ShouldReturnCorrectCount()
        {
            // Arrange
            const int count = 10;
            await this.SeedBankAccountAsync();
            for (int i = 1; i <= count; i++)
            {
                await this.dbContext.Transfers.AddAsync(new MoneyTransfer
                {
                    Account = await this.dbContext.Accounts.FirstOrDefaultAsync(),
                });
            }

            await this.dbContext.SaveChangesAsync();
            // Act
            var result =
                await this.moneyTransferService.GetAllMoneyTransfersForAccountAsync<MoneyTransferListingServiceModel>(SampleBankAccountId);

            // Assert
            result
                .Should()
                .HaveCount(count);
        }

        [Theory]
        [InlineData(null)]
        [InlineData(" dassdgdf")]
        [InlineData(" ")]
        [InlineData("!  ")]
        [InlineData("     10   ")]
        [InlineData("  sdgsfcx-arq12wasdasdzxxcvc   ")]
        public async Task GetLast10MoneyTransfersForUserAsync_WithInvalidUserId_ShouldReturnEmptyCollection(string userId)
        {
            // Arrange
            await this.SeedMoneyTransfersAsync();
            // Act
            var result =
                await this.moneyTransferService.GetLast10MoneyTransfersForUserAsync<MoneyTransferListingServiceModel>(userId);

            // Assert
            result
                .Should()
                .BeNullOrEmpty();
        }

        [Fact]
        public async Task GetLast10MoneyTransfersForUserAsync_WithValidUserId_ShouldReturnCollectionOfCorrectEntities()
        {
            // Arrange
            var model = await this.SeedMoneyTransfersAsync();
            // Act
            var result =
                await this.moneyTransferService.GetLast10MoneyTransfersForUserAsync<MoneyTransferListingServiceModel>(model.Account.UserId);

            // Assert
            result
                .Should()
                .AllBeAssignableTo<MoneyTransferListingServiceModel>()
                .And
                .Match<IEnumerable<MoneyTransferListingServiceModel>>(x => x.All(c => c.Source == model.Source));
        }

        [Fact]
        public async Task GetLast10MoneyTransfersForUserAsync_ShouldReturnOrderedByMadeOnCollection()
        {
            // Arrange
            await this.SeedBankAccountAsync();
            for (int i = 0; i < 22; i++)
            {
                await this.dbContext.Transfers.AddAsync(new MoneyTransfer
                {
                    Id = $"{SampleId}_{i}",
                    Account = await this.dbContext.Accounts.FirstOrDefaultAsync(),
                    MadeOn = DateTime.UtcNow.AddMinutes(i)
                });
            }

            await this.dbContext.SaveChangesAsync();
            // Act
            var result =
                await this.moneyTransferService.GetLast10MoneyTransfersForUserAsync<MoneyTransferListingServiceModel>(SampleBankAccountUserId);

            // Assert
            result
                .Should()
                .BeInDescendingOrder(x => x.MadeOn);
        }

        [Fact]
        public async Task GetLast10MoneyTransfersForUserAsync_ShouldReturnCorrectCount()
        {
            // Arrange
            const int count = 22;
            const int expectedCount = 10;
            await this.SeedBankAccountAsync();
            for (int i = 1; i <= count; i++)
            {
                await this.dbContext.Transfers.AddAsync(new MoneyTransfer
                {
                    Account = await this.dbContext.Accounts.FirstOrDefaultAsync(),
                });
            }

            await this.dbContext.SaveChangesAsync();
            // Act
            var result =
                await this.moneyTransferService.GetLast10MoneyTransfersForUserAsync<MoneyTransferListingServiceModel>(SampleBankAccountUserId);

            // Assert
            result
                .Should()
                .HaveCount(expectedCount);
        }

        #region privateMethods

        private async Task<MoneyTransfer> SeedMoneyTransfersAsync()
        {
            await this.SeedBankAccountAsync();
            var model = new MoneyTransfer
            {
                Id = SampleId,
                Description = SampleDescription,
                Amount = SampleAmount,
                Account = await this.dbContext.Accounts.FirstOrDefaultAsync(),
                Destination = SampleDestination,
                Source = SampleBankAccountId,
                SenderName = SampleSenderName,
                RecipientName = SampleRecipientName,
                MadeOn = DateTime.UtcNow,
            };

            await this.dbContext.Transfers.AddAsync(model);
            await this.dbContext.SaveChangesAsync();

            return model;
        }

        private async Task<BankAccount> SeedBankAccountAsync()
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
