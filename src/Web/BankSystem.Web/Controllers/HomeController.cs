using System.Diagnostics;
using BankSystem.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace BankSystem.Web.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using Models.BankAccount;
    using Services.Interfaces;
    using Services.Models.BankAccount;

    public class HomeController : Controller
    {
        private readonly IBankAccountService bankAccountService;
        private readonly IUserService userService;

        public HomeController(IBankAccountService bankAccountService, IUserService userService)
        {
            this.bankAccountService = bankAccountService;
            this.userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = await this.userService.GetUserIdAsyncByUsername(this.User.Identity.Name);
            var bankAccounts = (await this.bankAccountService.GetAllUserAccountsAsync<BankAccountIndexServiceModel>(userId))
                .Select(Mapper.Map<BankAccountIndexViewModel>)
                .ToList();

            var viewModel = new HomeViewModel
            {
                UserBankAccounts = bankAccounts,
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
