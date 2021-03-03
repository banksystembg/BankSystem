namespace BankSystem.Services.Tests.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using BankSystem.Models;
    using Card;
    using Common;
    using Data;
    using FluentAssertions;
    using Microsoft.EntityFrameworkCore;
    using Models.Card;
    using Xunit;

    public class CardServiceTests : BaseTest
    {
        public CardServiceTests()
        {
            this.dbContext = this.DatabaseInstance;
            this.cardService = new CardService(
                this.dbContext,
                new CardHelper(this.MockedBankConfiguration.Object),
                this.Mapper);
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
        public async Task CreateAsync_WithInvalidExpiryDate_Should_ReturnFalse(string expiryDate)
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
        }

        [Theory]
        [InlineData("03015135")]
        [InlineData("030124135")]
        [InlineData("01")]
        [InlineData("-10")]
        public async Task CreateAsync_WithInvalidExpiryDate_Should_Not_InsertInDatabase(string expiryDate)
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
            await this.cardService.CreateAsync(model);

            // Assert
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
        public async Task GetAllCardsAsync_WithInvalidUserId_Should_ReturnEmptyCollection(string userId)
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
        public async Task GetCountOfAllCardsOwnedByUserAsync_Should_ReturnCorrectCount()
        {
            // Arrange
            await this.SeedCardAsync();

            // Act
            var result = await this.cardService.GetCountOfAllCardsOwnedByUserAsync(SampleUserId);

            // Assert
            result
                .Should()
                .Be(await this.dbContext.Cards.CountAsync(c => c.UserId == SampleUserId));
        }

        [Theory]
        [InlineData(" ")]
        [InlineData(null)]
        [InlineData("209358)(%#*@)(%*#$)ET(WFI)SD")]
        [InlineData(" 1  4 10")]
        public async Task DeleteAsync_WithInvalidId_Should_ReturnFalse(string id)
        {
            // Arrange
            await this.SeedCardAsync();

            // Act
            var result = await this.cardService.DeleteAsync(id);

            // Assert
            result
                .Should()
                .BeFalse();
        }

        [Theory]
        [InlineData(" ")]
        [InlineData(null)]
        [InlineData("209358)(%#*@)(%*#$)ET(WFI)SD")]
        [InlineData(" 1  4 10")]
        public async Task DeleteAsync_WithInvalidId_Should_Not_DeleteCardFromDatabase(string id)
        {
            // Arrange
            await this.SeedCardAsync();

            // Act
            await this.cardService.DeleteAsync(id);

            // Assert
            this.dbContext
                .Cards
                .Should()
                .HaveCount(1);
        }

        [Fact]
        public async Task CreateAsync_WithInvalidModel_Should_ReturnFalse()
        {
            // Act
            var result = await this.cardService.CreateAsync(new CardCreateServiceModel());

            // Assert
            result
                .Should()
                .BeFalse();
        }

        [Fact]
        public async Task CreateAsync_WithInvalidModel_Should_Not_InsertInDatabase()
        {
            // Act
            await this.cardService.CreateAsync(new CardCreateServiceModel());

            // Assert
            this.dbContext
                .Cards
                .Should()
                .BeEmpty();
        }

        [Fact]
        public async Task CreateAsync_WithInvalidName_Should_ReturnFalse()
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
        }

        [Fact]
        public async Task CreateAsync_WithInvalidName_Should_Not_InsertInDatabase()
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
            await this.cardService.CreateAsync(model);

            // Assert
            this.dbContext
                .Cards
                .Should()
                .BeEmpty();
        }

        [Fact]
        public async Task CreateAsync_WithValidModel_Should_ReturnTrue()
        {
            // Arrange
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
        }

        [Fact]
        public async Task CreateAsync_WithValidModel_Should_InsertInDatabase()
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
            await this.cardService.CreateAsync(model);

            // Assert
            this.dbContext
                .Cards
                .Should()
                .HaveCount(dbCount + 1);
        }


        [Fact]
        public async Task DeleteAsync_WithValidId_Should_ReturnTrue()
        {
            // Arrange
            var model = await this.SeedCardAsync();

            // Act
            var result = await this.cardService.DeleteAsync(model.Id);

            // Assert
            result
                .Should()
                .BeTrue();
        }

        [Fact]
        public async Task DeleteAsync_WithValidId_Should_DeleteCardFromDatabase()
        {
            // Arrange
            var model = await this.SeedCardAsync();

            // Act
            await this.cardService.DeleteAsync(model.Id);

            // Assert
            this.dbContext
                .Cards
                .Should()
                .BeEmpty();
        }

        [Fact]
        public async Task GetAllCardsAsync_WithValidUserId_Should_ReturnCorrectCount()
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
        public async Task GetAllCardsAsync_WithValidUserId_Should_ReturnCorrectEntities()
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
        public async Task GetAsync_WithInvalidId_Should_ReturnNull()
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
        public async Task GetAsync_WithInvalidParameters_Should_ReturnNull()
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
        public async Task GetAsync_WithValidId_Should_ReturnCorrectEntity()
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
        public async Task GetAsync_WithValidParameters_Should_ReturnCorrectEntity()
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
    }
}