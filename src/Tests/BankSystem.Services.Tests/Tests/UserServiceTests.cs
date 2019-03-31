namespace BankSystem.Services.Tests.Tests
{
    using BankSystem.Models;
    using Data;
    using FluentAssertions;
    using Implementations;
    using Interfaces;
    using System.Threading.Tasks;
    using Xunit;

    public class UserServiceTests : BaseTest
    {
        private const string SampleUserId = "dsgsdg-dsg364tr-egdfb-jfd";
        private const string SampleUsername = "melik";

        private readonly BankSystemDbContext dbContext;
        private readonly IUserService userService;

        public UserServiceTests()
        {
            this.dbContext = base.DatabaseInstance;
            this.userService = new UserService(this.dbContext);
        }

        [Theory]
        [InlineData(" ")]
        [InlineData("asd  1 ")]
        [InlineData("     10   ")]
        [InlineData("5215@%*)%@")]
        public async Task GetUserIdByUsernameAsync_WithInvalidUsername_ShouldReturnNull(string username)
        {
            // Arrange
            await this.SeedUserAsync();
            // Act
            var result = await this.userService.GetUserIdByUsernameAsync(username);

            // Assert
            result
                .Should()
                .BeNull();
        }

        [Fact]
        public async Task GetUserIdByUsernameAsync_WithValidUsername_ShouldReturnNonEmptyString()
        {
            // Arrange
            await this.SeedUserAsync();
            // Act
            var result = await this.userService.GetUserIdByUsernameAsync(SampleUsername);

            // Assert
            result
                .Should()
                .NotBeNull()
                .And
                .BeAssignableTo<string>();
        }

        private async Task SeedUserAsync()
        {
            await this.dbContext.Users.AddAsync(new BankUser { Id = SampleUserId, UserName = SampleUsername, });
            await this.dbContext.SaveChangesAsync();
        }
    }
}
