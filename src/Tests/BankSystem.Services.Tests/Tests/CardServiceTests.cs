namespace BankSystem.Services.Tests.Tests
{
    using BankSystem.Models;
    using Common;
    using Data;
    using FluentAssertions;
    using Implementations;
    using Interfaces;
    using Microsoft.EntityFrameworkCore;
    using Models.Card;
    using System.Threading.Tasks;
    using Xunit;

    public class CardServiceTests : BaseTest
    {
        private const string SampleCardId = "de88436d-5761-4512-998b-40d8264aba37";
        private const string SampleUserId = "sdgsfcx-arq12wsdxcvc";
        private const string SampleAccountId = "ABC125trABSD1";
        private const string SampleNumber = "1017840221397613";
        private const string SampleSecurityCode = "685";
        private const string SampleExpiryDate = "08/22";
        private const string SampleName = "melik";

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

        [Fact]
        public async Task GetAsync_WithValidParameters_ShouldReturnCorrectEntity()
        {
            // Arrange
            var model = await this.SeedCard();

            // Act
            var result = await this.cardService.GetAsync<CardDetailsServiceModel>(model.Number, model.ExpiryDate,
                model.SecurityCode, model.User.FullName);

            // Assert
            result
                .Should()
                .NotBeNull()
                .And
                .Match(x => x.As<CardDetailsServiceModel>().Id == model.Id);
        }

        [Fact]
        public async Task GetAsync_WithInvalidParameters_ShouldReturnNull()
        {
            // Arrange
            var model = await this.SeedCard();

            // Act
            var result = await this.cardService.GetAsync<CardDetailsServiceModel>("wrong number", model.ExpiryDate,
                model.SecurityCode, model.User.FullName);

            // Assert
            result
                .Should()
                .BeNull();
        }

        [Fact]
        public async Task GetAsync_WithValidId_ShouldReturnCorrectEntity()
        {
            // Arrange
            var model = await this.SeedCard();
            var expectedId = model.Id;

            // Act
            var result = await this.cardService.GetAsync<CardDetailsServiceModel>(expectedId);

            // Assert
            result
                .Should()
                .NotBeNull()
                .And
                .Match(x => x.As<CardDetailsServiceModel>().Id == expectedId);
        }

        [Fact]
        public async Task GetAsync_WithInvalidId_ShouldReturnCorrectNull()
        {
            // Arrange
            var model = await this.SeedCard();

            // Act
            var result = await this.cardService.GetAsync<CardDetailsServiceModel>(null);

            // Assert
            result
                .Should()
                .BeNull();
        }

        #region privateMethods
        private async Task<Card> SeedCard()
        {
            await this.SeedUser();
            var model = new Card
            {
                Id = SampleCardId,
                Name = SampleName,
                Account = new BankAccount(),
                ExpiryDate = SampleExpiryDate,
                Number = SampleNumber,
                SecurityCode = SampleSecurityCode,
                User = await this.dbContext.Users.FirstOrDefaultAsync(),
            };

            await this.dbContext.Cards.AddAsync(model);
            await this.dbContext.SaveChangesAsync();

            return model;
        }

        private async Task SeedUser()
        {
            await this.dbContext.Users.AddAsync(new BankUser { Id = SampleUserId, FullName = SampleName, });
            await this.dbContext.SaveChangesAsync();
        }

        #endregion
    }
}
