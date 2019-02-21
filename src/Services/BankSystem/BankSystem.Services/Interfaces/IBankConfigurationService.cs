namespace BankSystem.Services.Interfaces
{
    public interface IBankConfigurationService
    {
        string UniqueIdentifier { get; }
        string Key { get; }
        string CentralApiPublicKey { get; }
    }
}
