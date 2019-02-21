namespace BankSystem.Services.Implementations
{
    using System;
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
                .ProjectTo<T>()
                .Take(10)
                .ToListAsync();

        public async Task<bool> CreateMoneyTransferAsync<T>(T model, bool isSending)
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
            if (!isSending)
            {
                userAccount.Balance += dbModel.Amount;
                dbModel.Amount = decimal.Parse($"{dbModel.Amount}");
                this.Context.Update(userAccount);
            }
            else
            {
                userAccount.Balance -= dbModel.Amount;
                var negativeAmount = Math.Abs(dbModel.Amount) * (-1);
                dbModel.Amount = negativeAmount;
                this.Context.Update(userAccount);
            }

            await this.Context.Transfers.AddAsync(dbModel);
            await this.Context.SaveChangesAsync();

            return true;
        }
    }
}
