namespace BankSystem.Services.Implementations
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using BankSystem.Models;
    using Data;
    using Interfaces;
    using Microsoft.EntityFrameworkCore;
    using Models.MoneyTransfer;

    public class MoneyTransferService : BaseService, IMoneyTransferService
    {
        public MoneyTransferService(BankSystemDbContext context)
            : base(context)
        {
        }

        public async Task<IEnumerable<T>> GetLast10MoneyTransfersForUserAsync<T>(string userId)
            where T : MoneyTransferBaseServiceModel
            => await this.Context
                .Transfers
                .Where(mt => mt.Account.UserId == userId)
                .OrderByDescending(mt => mt.MadeOn)
                .Take(10)
                .ProjectTo<T>()
                .ToArrayAsync();

        public async Task<bool> CreateMoneyTransferAsync<T>(T model)
            where T : MoneyTransferBaseServiceModel
        {
            if (!this.IsEntityStateValid(model))
            {
                return false;
            }

            var dbModel = Mapper.Map<MoneyTransfer>(model);
            var userAccount = await this.Context
                .Accounts
                .Where(u => u.Id == dbModel.AccountId)
                .SingleOrDefaultAsync();
            if (userAccount == null)
            {
                return false;
            }

            userAccount.Balance += dbModel.Amount;
            this.Context.Update(userAccount);

            await this.Context.Transfers.AddAsync(dbModel);
            await this.Context.SaveChangesAsync();

            return true;
        }
    }
}
