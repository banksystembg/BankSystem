namespace BankSystem.Services.Tests.Tests
{
    using FluentAssertions;
    using Implementations;
    using Interfaces;
    using Xunit;

    public class BankAccountUniqueIdHelperTests : BaseTest
    {
        private readonly IBankAccountUniqueIdHelper bankAccountUniqueIdHelper;

        public BankAccountUniqueIdHelperTests()
        {
            this.bankAccountUniqueIdHelper = new BankAccountUniqueIdHelper(this.MockedBankConfiguration.Object);
        }

        [Fact]
        public void GenerateAccountUniqueId_ShouldReturn12DigitsString()
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
        [InlineData("ABCJ98131785 ")]
        [InlineData("   asd  ")]
        public void IsUniqueIdValid_WithInvalidId_ShouldReturnFalse(string id)
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
        public void IsUniqueIdValid_WithValidId_ShouldReturnTrue(string id)
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
