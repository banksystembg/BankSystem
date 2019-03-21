namespace BankSystem.Services.Interfaces
{
    using Models.MoneyTransfer;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IMoneyTransferService
    {
        Task<IEnumerable<T>> GetAllMoneyTransfersAsync<T>(string userId)
            where T : MoneyTransferBaseServiceModel;

        Task<IEnumerable<T>> GetAllMoneyTransfersForAccountAsync<T>(string accountId)
             where T : MoneyTransferBaseServiceModel;

        Task<IEnumerable<T>> GetLast10MoneyTransfersForUserAsync<T>(string userId)
            where T : MoneyTransferBaseServiceModel;

        Task<bool> CreateMoneyTransferAsync<T>(T model)
            where T : MoneyTransferBaseServiceModel;
    }
}
