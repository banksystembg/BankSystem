namespace BankSystem.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Models.BankAccount;

    public interface IBankAccountService
    {
        Task<IEnumerable<T>> GetAllUserAccountsAsync<T>(string userId)
            where T : BankAccountBaseServiceModel;

        Task<string> CreateAsync(BankAccountCreateServiceModel model);

        Task<string> GetUserAccountUniqueId(string accountId);

        Task<string> GetAccountIdAsync(string uniqueId);

        Task<T> GetBankAccountAsync<T>(string id)
            where T : BankAccountBaseServiceModel;
    }
}
