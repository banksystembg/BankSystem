namespace BankSystem.Services.Implementations
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper.QueryableExtensions;
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

        public async Task<bool> TransferMoneyAsync<T>(T model) 
            where T : MoneyTransferBaseServiceModel
        {
            throw new System.NotImplementedException();
        }
    }
}
