namespace BankSystem.Services.Interfaces
{
    using Models.BankAccount;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IBankAccountService
    {
        Task<IEnumerable<T>> GetAllUserAccountsAsync<T>(string userId)
            where T : BankAccountBaseServiceModel;

        Task<string> CreateAsync(BankAccountCreateServiceModel model);

        Task<string> GetUserAccountUniqueId(string accountId);

        Task<T> GetBankAccountAsyncByUniqueId<T>(string uniqueId)
            where T : BankAccountBaseServiceModel;

        Task<string> GetBankAccountUserFullNameAsync(string id);

        Task<T> GetBankAccountAsync<T>(string id)
            where T : BankAccountBaseServiceModel;

        Task<bool> ChangeAccountNameAsync(string accountId, string newName);
        Task<BankAccountDetailsServiceModel> GetByAccountIdAsync(string id);
    }
}
