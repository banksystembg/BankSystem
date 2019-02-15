namespace BankSystem.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Models.MoneyTransfer;

    public interface IMoneyTransferService
    {
        Task<IEnumerable<T>> GetAllMoneyTransfersForGivenUserByUserIdAsync<T>(string userId)
            where T : MoneyTransferBaseServiceModel;
    }
}
