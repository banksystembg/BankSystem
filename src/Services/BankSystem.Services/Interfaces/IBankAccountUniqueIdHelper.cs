namespace BankSystem.Services.Interfaces
{
    public interface IBankAccountUniqueIdHelper
    {
        string GetUniqueBankIdentifier();
        string GenerateAccountUniqueId();
        bool IsUniqueIdValid(string id);
    }
}