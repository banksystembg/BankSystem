namespace BankSystem.Services.Interfaces
{
    public interface IBankConfigurationHelper
    {
        string UniqueIdentifier { get; }
        string Key { get; }
        string CentralApiPublicKey { get; }
        string First3CardDigits { get; }
        string BankName { get; }
        string CentralApiAddress { get; }
        string BankCountry { get; }
    }
}