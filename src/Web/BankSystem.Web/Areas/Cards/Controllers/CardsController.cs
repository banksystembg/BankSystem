namespace BankSystem.Web.Areas.Cards.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Models;
    using Services.Interfaces;
    using Services.Models.BankAccount;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class CardsController : BaseCardsController
    {
        private readonly IUserService userService;
        private readonly IBankAccountService bankAccountService;

        public CardsController(IUserService userService, IBankAccountService bankAccountService)
        {
            this.userService = userService;
            this.bankAccountService = bankAccountService;
        }

        public async Task<IActionResult> Create()
        {
            var userId = await this.userService.GetUserIdByUsernameAsync(this.User.Identity.Name);
            var userAccounts = await this.GetAllAccountsAsync(userId);

            var model = new CardCreateViewModel
            {
                BankAccounts = userAccounts,
            };

            return this.View(model);
        }

        private async Task<IEnumerable<SelectListItem>> GetAllAccountsAsync(string userId)
            => (await this.bankAccountService
                    .GetAllUserAccountsAsync<BankAccountIndexServiceModel>(userId))
                .Select(a => new SelectListItem { Text = a.Name, Value = a.Id })
                .ToArray();

    }
}
