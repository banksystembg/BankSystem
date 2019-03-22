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

        Task<T> GetByUniqueIdAsync<T>(string uniqueId)
            where T : BankAccountBaseServiceModel;

        Task<T> GetBankAccountAsync<T>(string id)
            where T : BankAccountBaseServiceModel;

        Task<bool> ChangeAccountNameAsync(string accountId, string newName);
    }
}
