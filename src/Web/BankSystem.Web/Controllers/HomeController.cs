namespace BankSystem.Web.Controllers
{
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using Areas.MoneyTransfers.Models;
    using AutoMapper;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Models.BankAccount;
    using Services.Interfaces;
    using Services.Models.BankAccount;
    using Services.Models.MoneyTransfer;

    [AllowAnonymous]
    public class HomeController : BaseController
    {
        private readonly IBankAccountService bankAccountService;
        private readonly IUserService userService;
        private readonly IMoneyTransferService moneyTransferService;

        public HomeController(
            IBankAccountService bankAccountService,
            IUserService userService,
            IMoneyTransferService moneyTransferService)
        {
            this.bankAccountService = bankAccountService;
            this.userService = userService;
            this.moneyTransferService = moneyTransferService;
        }


        public async Task<IActionResult> Index()
        {
            if (!this.User.Identity.IsAuthenticated)
            {
                return this.View("IndexGuest");
            }

            var userId = await this.userService.GetUserIdByUsernameAsync(this.User.Identity.Name);
            var bankAccounts =
                (await this.bankAccountService.GetAllUserAccountsAsync<BankAccountIndexServiceModel>(userId))
                .Select(Mapper.Map<BankAccountIndexViewModel>)
                .ToArray();
            var moneyTransfers = (await this.moneyTransferService
                    .GetLast10MoneyTransfersForUserAsync<MoneyTransferListingServiceModel>(userId))
                .Select(Mapper.Map<MoneyTransferListingViewModel>)
                .ToArray();

            var viewModel = new HomeViewModel
            {
                UserBankAccounts = bankAccounts,
                MoneyTransfers = moneyTransfers,
            };
            return this.View(viewModel);
        }

        public IActionResult Privacy()
        {
            return this.View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return this.View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
