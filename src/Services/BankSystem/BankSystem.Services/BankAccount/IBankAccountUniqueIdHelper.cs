namespace BankSystem.Services.BankAccount
{
    public interface IBankAccountUniqueIdHelper
    {
        string GenerateAccountUniqueId();

        bool IsUniqueIdValid(string id);
    }
}