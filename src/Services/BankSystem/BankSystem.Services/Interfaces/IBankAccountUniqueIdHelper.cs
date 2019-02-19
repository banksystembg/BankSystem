namespace BankSystem.Services.Interfaces
{
    public interface IBankAccountUniqueIdHelper
    {
        string GenerateAccountUniqueId();

        bool IsUniqueIdValid(string id);

        string AppId { get; }

        string ApiKey { get; }
    }
}