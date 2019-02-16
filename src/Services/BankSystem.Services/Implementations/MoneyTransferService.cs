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

        public async Task<IEnumerable<T>> GetAllMoneyTransfersForGivenUserByUserIdAsync<T>(string userId)
            where T : MoneyTransferBaseServiceModel
            => await this.Context
                .Transfers
                .Where(mt => mt.Account.UserId == userId)
                .ProjectTo<T>()
                .ToListAsync();
    }
}
