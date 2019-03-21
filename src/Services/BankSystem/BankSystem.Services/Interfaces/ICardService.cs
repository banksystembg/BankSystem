namespace BankSystem.Services.Interfaces
{
    using Models.Card;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ICardService
    {
        Task<string> CreateAsync(CardCreateServiceModel model);

        Task<IEnumerable<T>> GetAllCardsAsync<T>(string userId)
            where T : CardBaseServiceModel;

        Task<bool> DeleteAsync(string id);
    }
}
