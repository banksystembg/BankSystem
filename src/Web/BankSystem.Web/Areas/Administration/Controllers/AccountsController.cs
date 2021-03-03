namespace BankSystem.Web.Areas.Administration.Controllers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using Infrastructure.Extensions;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Services.BankAccount;
    using Services.Models.BankAccount;

    public class AccountsController : BaseAdministrationController
    {
        private const int AccountsPerPage = 20;

        private readonly IBankAccountService bankAccountService;
        private readonly IMapper mapper;

        public AccountsController(IBankAccountService bankAccountService, IMapper mapper)
        {
            this.bankAccountService = bankAccountService;
            this.mapper = mapper;
        }

        public async Task<IActionResult> Index(int pageIndex = 1)
        {
            pageIndex = Math.Max(1, pageIndex);

            var allAccounts = (await this.bankAccountService.GetAccountsAsync<BankAccountDetailsServiceModel>(pageIndex, AccountsPerPage))
                .Select(this.mapper.Map<BankAccountListingViewModel>)
                .ToPaginatedList(await this.bankAccountService.GetCountOfAccountsAsync(), pageIndex, AccountsPerPage);

            var transfers = new AllBankAccountsListViewModel
            {
                BankAccounts = allAccounts
            };

            return this.View(transfers);
        }
    }
}