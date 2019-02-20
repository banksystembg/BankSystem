namespace CentralApi.Services.Implementations
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Data;
    using Interfaces;
    using Microsoft.EntityFrameworkCore;

    public class BankService : BaseService, IBanksService
    {
        public BankService(CentralApiDbContext context)
            : base(context)
        {
        }

        public async Task<bool> GetBankAsync(string bankName, string swiftCode, string bankCountry)
        {
            var bank = await this.Context
                .Banks
                .Where(b => string.Equals(b.Name, bankName, StringComparison.CurrentCultureIgnoreCase) &&
                            string.Equals(b.SwiftCode, swiftCode, StringComparison.CurrentCultureIgnoreCase)
                            && string.Equals(b.Location, bankCountry, StringComparison.CurrentCultureIgnoreCase))
                .SingleOrDefaultAsync();

            return bank != null;
        }
    }
}
