namespace BankSystem.Services.BankAccount
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using BankSystem.Models;
    using Data;
    using Microsoft.EntityFrameworkCore;
    using Models.BankAccount;

    public class BankAccountService : BaseService, IBankAccountService
    {
        private readonly IBankAccountUniqueIdHelper uniqueIdHelper;
        private readonly IMapper mapper;
        
        public BankAccountService(BankSystemDbContext context, IBankAccountUniqueIdHelper uniqueIdHelper, IMapper mapper)
            : base(context)
        {
            this.uniqueIdHelper = uniqueIdHelper;
            this.mapper = mapper;
        }

        public async Task<string> CreateAsync(BankAccountCreateServiceModel model)
        {
            if (!this.IsEntityStateValid(model) ||
                !this.Context.Users.Any(u => u.Id == model.UserId))
            {
                return null;
            }

            string generatedUniqueId;

            do
            {
                generatedUniqueId = this.uniqueIdHelper.GenerateAccountUniqueId();
            } while (this.Context.Accounts.Any(a => a.UniqueId == generatedUniqueId));

            model.Name ??= generatedUniqueId;

            var dbModel = this.mapper.Map<BankAccount>(model);
            dbModel.UniqueId = generatedUniqueId;

            await this.Context.Accounts.AddAsync(dbModel);
            await this.Context.SaveChangesAsync();

            return dbModel.Id;
        }

        public async Task<T> GetByUniqueIdAsync<T>(string uniqueId)
            where T : BankAccountBaseServiceModel
            => await this.Context
                .Accounts
                .AsNoTracking()
                .Where(a => a.UniqueId == uniqueId)
                .ProjectTo<T>(this.mapper.ConfigurationProvider)
                .SingleOrDefaultAsync();

        public async Task<T> GetByIdAsync<T>(string id)
            where T : BankAccountBaseServiceModel
            => await this.Context
                .Accounts
                .AsNoTracking()
                .Where(a => a.Id == id)
                .ProjectTo<T>(this.mapper.ConfigurationProvider)
                .SingleOrDefaultAsync();

        public async Task<bool> ChangeAccountNameAsync(string accountId, string newName)
        {
            var account = await this.Context
                .Accounts
                .FindAsync(accountId);
            if (account == null)
            {
                return false;
            }

            account.Name = newName;
            this.Context.Update(account);
            await this.Context.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<T>> GetAllAccountsByUserIdAsync<T>(string userId)
            where T : BankAccountBaseServiceModel
            => await this.Context
                .Accounts
                .AsNoTracking()
                .Where(a => a.UserId == userId)
                .ProjectTo<T>(this.mapper.ConfigurationProvider)
                .ToArrayAsync();

        public async Task<IEnumerable<T>> GetAccountsAsync<T>(int pageIndex = 1, int count = int.MaxValue)
            where T : BankAccountBaseServiceModel
            => await this.Context
                .Accounts
                .AsNoTracking()
                .Skip((pageIndex - 1) * count)
                .Take(count)
                .ProjectTo<T>(this.mapper.ConfigurationProvider)
                .ToArrayAsync();

        public async Task<int> GetCountOfAccountsAsync()
            => await this.Context
                .Accounts
                .CountAsync();
    }
}