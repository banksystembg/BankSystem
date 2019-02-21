namespace CentralApi.Services.Implementations
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper.QueryableExtensions;
    using Data;
    using Interfaces;
    using Microsoft.EntityFrameworkCore;
    using Models.Banks;

    public class BankService : BaseService, IBanksService
    {
        public BankService(CentralApiDbContext context)
            : base(context)
        {
        }

        public async Task<T> GetBankAsync<T>(string bankName, string swiftCode, string bankCountry)
            where T : BankBaseServiceModel
            => await this.Context
                .Banks
                .Where(b => string.Equals(b.Name, bankName, StringComparison.CurrentCultureIgnoreCase) &&
                            string.Equals(b.SwiftCode, swiftCode, StringComparison.CurrentCultureIgnoreCase)
                            && string.Equals(b.Location, bankCountry, StringComparison.CurrentCultureIgnoreCase))
                .ProjectTo<T>()
                .SingleOrDefaultAsync();
    }
}
