namespace BankSystem.Services.Interfaces
{
    using System.Threading.Tasks;

    public interface IUserService
    {
        Task<string> GetUserIdAsyncByUsername(string username);
    }
}
