namespace BankSystem.Services.Interfaces
{
    using Models.BankAccount;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IBankAccountService
    {
        Task<IEnumerable<T>> GetAllUserAccountsAsync<T>(string userId)
            where T : BankAccountBaseServiceModel;

        Task<string> CreateAsync(BankAccountCreateServiceModel model);

        Task<string> GetUserAccountUniqueId(string accountId);

        Task<string> GetAccountIdAsync(string uniqueId);

        Task<string> GetAccountIdAsync(string cardNumber, DateTime cardExpiryDate, int cardSecurityCode, string cardOwner);

        Task<T> GetBankAccountAsync<T>(string id)
            where T : BankAccountBaseServiceModel;

        Task<bool> ChangeAccountNameAsync(string accountId, string newName);
    }
}
