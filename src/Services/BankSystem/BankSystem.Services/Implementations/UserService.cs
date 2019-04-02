namespace BankSystem.Services.Implementations
{
    using System.Threading.Tasks;
    using Data;
    using Interfaces;
    using Microsoft.EntityFrameworkCore;

    public class UserService : BaseService, IUserService
    {
        public UserService(BankSystemDbContext context)
            : base(context)
        {
        }

        public async Task<string> GetUserIdByUsernameAsync(string username)
        {
            var user = await this.Context
                .Users
                .SingleOrDefaultAsync(u => u.UserName == username);

            return user?.Id;
        }

        public async Task<string> GetAccountOwnerFullnameAsync(string userId)
        {
            var user = await this.Context
                .Users
                .SingleOrDefaultAsync(u => u.Id == userId);

            return user?.FullName;
        }
    }
}