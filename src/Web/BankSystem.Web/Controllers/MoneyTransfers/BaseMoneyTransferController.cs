namespace BankSystem.Web.Controllers.MoneyTransfers
{
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using Models.BankAccount;
    using Services.Interfaces;
    using Services.Models.BankAccount;

    public abstract class BaseMoneyTransferController : BaseController
    {
        private readonly IBankAccountService bankAccountService;

        protected BaseMoneyTransferController(IBankAccountService bankAccountService)
        {
            this.bankAccountService = bankAccountService;
        }

        protected async Task<OwnBankAccountListingViewModel[]> GetAllAccountsAsync(string userId)
        {
            return (await this.bankAccountService
                    .GetAllUserAccountsAsync<BankAccountIndexServiceModel>(userId))
                .Select(Mapper.Map<OwnBankAccountListingViewModel>)
                .ToArray();
        }
    }
}
