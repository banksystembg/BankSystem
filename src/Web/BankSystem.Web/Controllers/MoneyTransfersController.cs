namespace BankSystem.Web.Controllers
{
    using System;
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
        private readonly IMoneyTransferService moneyTransferService;
        private readonly IBankAccountService bankAccountService;
        private readonly IUserService userService;

        public MoneyTransfersController(
            IMoneyTransferService moneyTransferService,
            IBankAccountService bankAccountService,
            IUserService userService)
        {
            this.moneyTransferService = moneyTransferService;
            this.bankAccountService = bankAccountService;
            this.userService = userService;
        }

        public async Task<IActionResult> Create()
        {
            var userAccounts = await this.GetAllUserAccountsAsync();
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

        [HttpPost]
        public async Task<IActionResult> Create(MoneyTransferCreateBindingModel model)
        {
            throw new NotImplementedException();
        }

        private async Task<IEnumerable<SelectListItem>> GetAllUserAccountsAsync()
        {
            var userId = await this.userService.GetUserIdByUsernameAsync(this.User.Identity.Name);
            var userAccounts = await this.bankAccountService
                .GetAllUserAccountsAsync<BankAccountIndexServiceModel>(userId);

            return userAccounts
                .Select(a => new SelectListItem { Text = a.Name, Value = a.Id })
                .ToArray();
        }
    }
}
