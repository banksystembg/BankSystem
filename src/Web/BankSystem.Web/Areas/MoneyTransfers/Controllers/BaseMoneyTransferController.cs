namespace BankSystem.Web.Areas.MoneyTransfers.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using Microsoft.AspNetCore.Mvc;
    using Services.Interfaces;
    using Services.Models.BankAccount;
    using Web.Controllers;
    using Web.Models.BankAccount;

    [Area("MoneyTransfers")]
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
