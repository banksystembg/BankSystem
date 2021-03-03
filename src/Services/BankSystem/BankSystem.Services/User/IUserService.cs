namespace BankSystem.Services.User
{
    using System.Threading.Tasks;

    public interface IUserService
    {
        Task<string> GetUserIdByUsernameAsync(string username);
        Task<string> GetAccountOwnerFullnameAsync(string userId);
    }
}