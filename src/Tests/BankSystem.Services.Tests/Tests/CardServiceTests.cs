namespace BankSystem.Services.Tests.Tests
{
    using BankSystem.Models;
    using Common;
    using Data;
    using FluentAssertions;
    using Implementations;
    using Interfaces;
    using Models.Card;
    using System.Threading.Tasks;
    using Xunit;

    public class CardServiceTests : BaseTest
    {
        private const string SampleName = "test name";
        private const string SampleUserId = "sdgsfcx-arq12wsdxcvc";
        private const string SampleAccountId = "ABC125trABSD1";
        private const string SampleExpiryDate = "08/22";

        private readonly BankSystemDbContext dbContext;
        private readonly ICardService cardService;

        public CardServiceTests()
        {
            this.dbContext = base.DatabaseInstance;
            this.cardService = new CardService(this.dbContext, new CardHelper(new BankConfigurationHelper(this.MockedBankConfiguration.Object)));
        }

        [Fact]
        public async Task CreateAsync_WithInvalidModel_ShouldReturnFalse()
        {
            // Act
            var result = await this.cardService.CreateAsync(new CardCreateServiceModel());

            // Assert
            result
                .Should()
                .BeFalse();
        }

        [Fact]
        public async Task CreateAsync_WithInvalidName_ShouldReturnFalse()
        {
            // Set invalid name
            var model = new CardCreateServiceModel
            {
                Name = new string('m', ModelConstants.Card.NameMaxLength + 1),
                UserId = SampleUserId,
                AccountId = SampleAccountId,
                ExpiryDate = SampleExpiryDate,
            };

            // Act
            var result = await this.cardService.CreateAsync(model);

            // Assert
            result
                .Should()
                .BeFalse();
        }

        [Theory]
        [InlineData("03015135")]
        [InlineData("030124135")]
        [InlineData("01")]
        [InlineData("-10")]
        public async Task CreateAsync_WithInvalidExpiryDate_ShouldReturnFalse(string expiryDate)
        {
            // Set invalid expiryDate
            var model = new CardCreateServiceModel
            {
                Name = SampleName,
                UserId = SampleUserId,
                AccountId = SampleAccountId,
                ExpiryDate = expiryDate,
            };

            // Act
            var result = await this.cardService.CreateAsync(model);

            // Assert
            result
                .Should()
                .BeFalse();
        }

        [Fact]
        public async Task CreateAsync_WithValidModel_ShouldReturnTrue()
        {
            await this.SeedUser();
            // Arrange
            var model = new CardCreateServiceModel
            {
                Name = SampleName,
                UserId = SampleUserId,
                AccountId = SampleAccountId,
                ExpiryDate = SampleExpiryDate,
            };

            // Act
            var result = await this.cardService.CreateAsync(model);

            // Assert
            result
                .Should()
                .BeTrue();
        }

        private async Task SeedUser()
        {
            await this.dbContext.Users.AddAsync(new BankUser { Id = SampleUserId });
            await this.dbContext.SaveChangesAsync();
        }
    }
}
