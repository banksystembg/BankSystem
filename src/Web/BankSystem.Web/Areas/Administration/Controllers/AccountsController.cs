namespace BankSystem.Web.Areas.Administration.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using Infrastructure.Extensions;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Services.Interfaces;
    using Services.Models.BankAccount;

    public class AccountsController : BaseAdministrationController
    {
        private const int AccountsPerPage = 20;

        private readonly IBankAccountService bankAccountService;

        public AccountsController(IBankAccountService bankAccountService)
        {
            this.bankAccountService = bankAccountService;
        }

        public async Task<IActionResult> Index(int pageIndex = 1)
        {
            var allAccounts = (await this.bankAccountService.GetAllAccountsAsync<BankAccountDetailsServiceModel>())
                .Select(Mapper.Map<BankAccountListingViewModel>)
                .ToPaginatedList(pageIndex, AccountsPerPage);

            var transfers = new AllBankAccountsListViewModel
            {
                BankAccounts = allAccounts
            };

            return this.View(transfers);
        }
    }
}