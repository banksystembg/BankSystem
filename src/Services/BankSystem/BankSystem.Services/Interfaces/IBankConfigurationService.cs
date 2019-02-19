namespace BankSystem.Services.Interfaces
{
    public interface IBankConfigurationService
    {
        string UniqueIdentifier { get; }
        string AppId { get; }
        string ApiKey { get; }
    }
}
