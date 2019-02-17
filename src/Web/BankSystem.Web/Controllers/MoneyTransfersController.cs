namespace BankSystem.Web.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Common;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Models.MoneyTransfer;
    using Services.Interfaces;
    using Services.Models.BankAccount;

    [Authorize]
    public class MoneyTransfersController : BaseController
    {
        private readonly IBankAccountService bankAccountService;
        private readonly IUserService userService;

        public MoneyTransfersController(IBankAccountService bankAccountService, IUserService userService)
        {
            this.bankAccountService = bankAccountService;
            this.userService = userService;
        }

        public async Task<IActionResult> Create()
        {
            var userId = await this.userService.GetUserIdByUsernameAsync(this.User.Identity.Name);
            if (userId == null)
            {
                this.ShowErrorMessage(NotificationMessages.TryAgainLaterError);
                return this.RedirectToHome();
            }
            var userAccounts = await this.GetAllUserAccountsAsync(userId);
            if (!userAccounts.Any())
            {
                this.ShowErrorMessage(NotificationMessages.NoAccountsError);
                return this.RedirectToHome();
            }

            var model = new MoneyTransferCreateBindingModel
            {
                UserAccounts = userAccounts,
            };

            return this.View(model);
        }

        private async Task<IEnumerable<SelectListItem>> GetAllUserAccountsAsync(string userId)
        {
            var userAccounts = await this.bankAccountService
                .GetAllUserAccountsAsync<BankAccountIndexServiceModel>(userId);

            return userAccounts
                .Select(a => new SelectListItem { Text = a.Name, Value = a.Id })
                .ToArray();
        }
    }
}
