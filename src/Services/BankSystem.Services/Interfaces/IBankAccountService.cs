namespace BankSystem.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IBankAccountService
    {
        Task<IEnumerable<string>> GetAllUserAccountsAsync(string userId);
    }
}
