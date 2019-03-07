namespace CentralApi.Services.Implementations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper.QueryableExtensions;
    using Data;
    using Interfaces;
    using Microsoft.EntityFrameworkCore;
    using Models.Banks;

    public class BanksService : BaseService, IBanksService
    {
        public BanksService(CentralApiDbContext context)
            : base(context)
        {
        }

        public async Task<T> GetBankAsync<T>(string bankName, string swiftCode, string bankCountry)
            where T : BankBaseServiceModel
        {
            var bank = await this.Context
                .Banks
                .Where(b => string.Equals(b.Name, bankName, StringComparison.CurrentCultureIgnoreCase) &&
                            string.Equals(b.SwiftCode, swiftCode, StringComparison.CurrentCultureIgnoreCase)
                            && string.Equals(b.Location, bankCountry, StringComparison.CurrentCultureIgnoreCase))
                .ProjectTo<T>()
                .SingleOrDefaultAsync();

            return bank;
        }

        public async Task<IEnumerable<T>> GetAllBanksSupportingPaymentsAsync<T>()
            where T : BankBaseServiceModel
        {
            var banks = await this.Context.Banks
                .Where(b => b.PaymentUrl != null)
                .OrderBy(b => b.Location)
                .ThenBy(b => b.Name)
                .ProjectTo<T>()
                .ToArrayAsync();

            return banks;
        }

        public async Task<T> GetBankByIdAsync<T>(string id)
            where T : BankBaseServiceModel
        {
            return await this.Context.Banks
                .Where(b => b.Id == id)
                .ProjectTo<T>()
                .SingleOrDefaultAsync();
        }
    }
}