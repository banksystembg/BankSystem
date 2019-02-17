namespace BankSystem.Services.Interfaces
{
    public interface IBankAccountUniqueIdHelper
    {
        string GenerateAccountUniqueId();
        bool IsUniqueIdValid(string id);
    }
}