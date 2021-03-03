namespace BankSystem.Web.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;
    using Areas.MoneyTransfers.Models;
    using AutoMapper;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Models.BankAccount;
    using Services.BankAccount;
    using Services.Models.BankAccount;
    using Services.Models.MoneyTransfer;
    using Services.MoneyTransfer;

    [AllowAnonymous]
    public class HomeController : BaseController
    {
        private readonly IBankAccountService bankAccountService;
        private readonly IMoneyTransferService moneyTransferService;
        private readonly IMapper mapper;

        public HomeController(
            IBankAccountService bankAccountService,
            IMoneyTransferService moneyTransferService,
            IMapper mapper)
        {
            this.bankAccountService = bankAccountService;
            this.moneyTransferService = moneyTransferService;
            this.mapper = mapper;
        }


        public async Task<IActionResult> Index()
        {
            if (!this.User.Identity.IsAuthenticated)
            {
                return this.View("IndexGuest");
            }

            var userId = this.GetCurrentUserId();

            var bankAccounts =
                (await this.bankAccountService.GetAllAccountsByUserIdAsync<BankAccountIndexServiceModel>(userId))
                .Select(this.mapper.Map<BankAccountIndexViewModel>)
                .ToArray();
            var moneyTransfers = (await this.moneyTransferService
                    .GetLast10MoneyTransfersForUserAsync<MoneyTransferListingServiceModel>(userId))
                .Select(this.mapper.Map<MoneyTransferListingDto>)
                .ToArray();

            var viewModel = new HomeViewModel
            {
                UserBankAccounts = bankAccounts,
                MoneyTransfers = moneyTransfers
            };

            return this.View(viewModel);
        }
    }
}