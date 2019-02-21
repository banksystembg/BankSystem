namespace BankSystem.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Models.MoneyTransfer;

    public interface IMoneyTransferService
    {
        Task<IEnumerable<T>> GetLast10MoneyTransfersForUserAsync<T>(string userId)
            where T : MoneyTransferBaseServiceModel;

        Task<bool> CreateMoneyTransferAsync<T>(T model, bool isSending = false)
            where T : MoneyTransferBaseServiceModel;
    }
}
