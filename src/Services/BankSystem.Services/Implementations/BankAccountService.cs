namespace BankSystem.Services.Implementations
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper.QueryableExtensions;
    using Data;
    using Interfaces;
    using Microsoft.EntityFrameworkCore;
    using Models.BankAccount;

    public class BankAccountService : BaseService, IBankAccountService
    {
        public BankAccountService(BankSystemDbContext context) 
            : base(context)
        {
        }

        public async Task<IEnumerable<T>> GetAllUserAccountsAsync<T>(string userId) 
            where T : BankAccountBaseServiceModel
            => await this.Context
                .Accounts
                .Where(a => a.UserId == userId)
                .ProjectTo<T>()
                .ToListAsync();
    }
}
