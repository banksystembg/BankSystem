namespace BankSystem.Services.Tests.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using BankSystem.Models;
    using Common;
    using Data;
    using FluentAssertions;
    using Implementations;
    using Interfaces;
    using Microsoft.EntityFrameworkCore;
    using Models.Card;
    using Xunit;

    public class CardServiceTests : BaseTest
    {
        public CardServiceTests()
        {
            this.dbContext = this.DatabaseInstance;
            this.cardService = new CardService(this.dbContext, new CardHelper(this.MockedBankConfiguration.Object));
        }

        private const string SampleCardId = "de88436d-5761-4512-998b-40d8264aba37";
        private const string SampleUserId = "sdgsfcx-arq12wsdxcvc";
        private const string SampleAccountId = "ABC125trABSD1";
        private const string SampleNumber = "1017840221397613";
        private const string SampleSecurityCode = "685";
        private const string SampleExpiryDate = "08/22";
        private const string SampleName = "melik";

        private readonly BankSystemDbContext dbContext;
        private readonly ICardService cardService;

        [Theory]
        [InlineData("03015135")]
        [InlineData("030124135")]
        [InlineData("01")]
        [InlineData("-10")]
        public async Task CreateAsync_WithInvalidExpiryDate_ShouldReturnFalse_And_NotInsertInDatabase(string expiryDate)
        {
            // Set invalid expiryDate
            var model = new CardCreateServiceModel
            {
                Name = SampleName,
                UserId = SampleUserId,
                AccountId = SampleAccountId,
                ExpiryDate = expiryDate
            };

            // Act
            var result = await this.cardService.CreateAsync(model);

            // Assert
            result
                .Should()
                .BeFalse();

            this.dbContext
                .Cards
                .Should()
                .BeEmpty();
        }

        [Theory]
        [InlineData(" ")]
        [InlineData(null)]
        [InlineData("!")]
        [InlineData(" sdgsfcx-arq12wsdxcvc")]
        public async Task GetAllCardsAsync_WithInvalidUserId_ShouldReturnEmptyCollection(string userId)
        {
            // Arrange
            await this.SeedCardAsync();

            // Act
            var result = await this.cardService.GetCardsAsync<CardDetailsServiceModel>(userId);

            // Assert
            result
                .Should()
                .BeNullOrEmpty();
        }

        [Fact]
        public async Task GetCountOfAllCardsOwnedByUserAsync_ShouldReturnCorrectCount()
        {
            // Arrange
            await this.SeedCardAsync();

            // Act
            var result = await this.cardService.GetCountOfAllCardsOwnedByUserAsync(SampleUserId);

            // Assert
            result
                .Should()
                .Be(await this.dbContext.Cards.CountAsync(c=> c.UserId == SampleUserId));
        }

        [Theory]
        [InlineData(" ")]
        [InlineData(null)]
        [InlineData("209358)(%#*@)(%*#$)ET(WFI)SD")]
        [InlineData(" 1  4 10")]
        public async Task DeleteAsync_WithInvalidId_ShouldReturnFalse_And_NotDeleteCardFromDatabase(string id)
        {
            // Arrange
            await this.SeedCardAsync();

            // Act
            var result = await this.cardService.DeleteAsync(id);

            // Assert
            result
                .Should()
                .BeFalse();

            this.dbContext
                .Cards
                .Should()
                .HaveCount(1);
        }

        private async Task<Card> SeedCardAsync()
        {
            await this.SeedUserAsync();
            var model = new Card
            {
                Id = SampleCardId,
                Name = SampleName,
                Account = new BankAccount(),
                ExpiryDate = SampleExpiryDate,
                Number = SampleNumber,
                SecurityCode = SampleSecurityCode,
                User = await this.dbContext.Users.FirstOrDefaultAsync()
            };

            await this.dbContext.Cards.AddAsync(model);
            await this.dbContext.SaveChangesAsync();

            return model;
        }

        private async Task SeedUserAsync()
        {
            await this.dbContext.Users.AddAsync(new BankUser { Id = SampleUserId, FullName = SampleName });
            await this.dbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task CreateAsync_WithInvalidModel_ShouldReturnFalse_And_NotInsertInDatabase()
        {
            // Act
            var result = await this.cardService.CreateAsync(new CardCreateServiceModel());

            // Assert
            result
                .Should()
                .BeFalse();

            this.dbContext
                .Cards
                .Should()
                .BeEmpty();
        }

        [Fact]
        public async Task CreateAsync_WithInvalidName_ShouldReturnFalse_And_NotInsertInDatabase()
        {
            // Set invalid name
            var model = new CardCreateServiceModel
            {
                Name = new string('m', ModelConstants.Card.NameMaxLength + 1),
                UserId = SampleUserId,
                AccountId = SampleAccountId,
                ExpiryDate = SampleExpiryDate
            };

            // Act
            var result = await this.cardService.CreateAsync(model);

            // Assert
            result
                .Should()
                .BeFalse();

            this.dbContext
                .Cards
                .Should()
                .BeEmpty();
        }

        [Fact]
        public async Task CreateAsync_WithValidModel_ShouldReturnTrue_And_InsertInDatabase()
        {
            // Arrange
            var dbCount = this.dbContext.Accounts.Count();

            await this.SeedUserAsync();
            var model = new CardCreateServiceModel
            {
                Name = SampleName,
                UserId = SampleUserId,
                AccountId = SampleAccountId,
                ExpiryDate = SampleExpiryDate
            };

            // Act
            var result = await this.cardService.CreateAsync(model);

            // Assert
            result
                .Should()
                .BeTrue();

            this.dbContext
                .Cards
                .Should()
                .HaveCount(dbCount + 1);
        }


        [Fact]
        public async Task DeleteAsync_WithValidId_ShouldReturnTrue_And_DeleteCardFromDatabase()
        {
            // Arrange
            var model = await this.SeedCardAsync();

            // Act
            var result = await this.cardService.DeleteAsync(model.Id);

            // Assert
            result
                .Should()
                .BeTrue();

            this.dbContext
                .Cards
                .Should()
                .BeEmpty();
        }

        [Fact]
        public async Task GetAllCardsAsync_WithValidUserId_ShouldReturnCorrectCount()
        {
            // Arrange
            var model = await this.SeedCardAsync();

            // Act
            var result = await this.cardService.GetCardsAsync<CardDetailsServiceModel>(model.UserId);

            // Assert
            result
                .Should()
                .HaveCount(1);

        }

        [Fact]
        public async Task GetAllCardsAsync_WithValidUserId_ShouldReturnCorrectEntities()
        {
            // Arrange
            var model = await this.SeedCardAsync();

            // Act
            var result = await this.cardService.GetCardsAsync<CardDetailsServiceModel>(model.UserId);

            // Assert
            result
                .Should()
                .AllBeAssignableTo<CardDetailsServiceModel>()
                .And
                .Match<IEnumerable<CardDetailsServiceModel>>(x => x.All(c => c.UserId == model.UserId));

        }

        [Fact]
        public async Task GetAsync_WithInvalidId_ShouldReturnNull()
        {
            // Arrange
            await this.SeedCardAsync();

            // Act
            var result = await this.cardService.GetAsync<CardDetailsServiceModel>(null);

            // Assert
            result
                .Should()
                .BeNull();
        }

        [Fact]
        public async Task GetAsync_WithInvalidParameters_ShouldReturnNull()
        {
            // Arrange
            var model = await this.SeedCardAsync();

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
            var model = await this.SeedCardAsync();
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
        public async Task GetAsync_WithValidParameters_ShouldReturnCorrectEntity()
        {
            // Arrange
            var model = await this.SeedCardAsync();

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
    }
}
