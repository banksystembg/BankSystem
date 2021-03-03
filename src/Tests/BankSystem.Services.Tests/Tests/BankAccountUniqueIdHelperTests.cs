namespace BankSystem.Services.Tests.Tests
{
    using BankAccount;
    using FluentAssertions;
    using Xunit;

    public class BankAccountUniqueIdHelperTests : BaseTest
    {
        private readonly IBankAccountUniqueIdHelper bankAccountUniqueIdHelper;

        public BankAccountUniqueIdHelperTests()
            => this.bankAccountUniqueIdHelper = new BankAccountUniqueIdHelper(this.MockedBankConfiguration.Object);

        [Fact]
        public void GenerateAccountUniqueId_Should_Return12DigitsString()
        {
            // Act
            var result = this.bankAccountUniqueIdHelper.GenerateAccountUniqueId();

            // Assert
            result
                .Should()
                .BeAssignableTo<string>();

            result
                .Should()
                .HaveLength(12);
        }

        [Theory]
        [InlineData(" ABCJ98131783")]
        [InlineData("ABC J98  1317 85 ")]
        [InlineData("ABCJz568235t89")]
        [InlineData("ABCJ98131786")]
        [InlineData("ABCA98131785")]
        [InlineData("abcJ98131785")]
        [InlineData("abcJ981317857")]
        [InlineData("ABAJ98131785")]
        [InlineData("ABCJA8131785")]
        public void IsUniqueIdValid_WithInvalidId_Should_ReturnFalse(string id)
        {
            // Act
            var result = this.bankAccountUniqueIdHelper.IsUniqueIdValid(id);

            // Assert
            result
                .Should()
                .BeFalse();
        }

        [Theory]
        [InlineData("ABCY12054790")]
        [InlineData("ABCY52630755")]
        [InlineData("ABCJ98131785")]
        public void IsUniqueIdValid_WithValidId_Should_ReturnTrue(string id)
        {
            // Act
            var result = this.bankAccountUniqueIdHelper.IsUniqueIdValid(id);

            // Assert
            result
                .Should()
                .BeTrue();
        }
    }
}
