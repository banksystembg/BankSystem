namespace BankSystem.Web.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using Common;
    using Infrastructure.Filters;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Models.InternalMoneyTransfers;
    using Services.Interfaces;
    using Services.Models.BankAccount;
    using Services.Models.MoneyTransfer;

    [Authorize]
    public class InternalMoneyTransfersController : BaseController
    {
        private readonly IMoneyTransferService moneyTransferService;
        private readonly IBankAccountService bankAccountService;
        private readonly IUserService userService;

        public InternalMoneyTransfersController(IMoneyTransferService moneyTransferService,
            IBankAccountService bankAccountService,
            IUserService userService)
        {
            this.moneyTransferService = moneyTransferService;
            this.bankAccountService = bankAccountService;
            this.userService = userService;
        }


        public async Task<IActionResult> Create()
        {
            var userId = await this.userService.GetUserIdByUsernameAsync(this.User.Identity.Name);
            var userAccounts = await this.GetAllUserAccountsAsync(userId);

            if (!userAccounts.Any())
            {
                this.ShowErrorMessage(NotificationMessages.NoAccountsError);
                return this.RedirectToHome();
            }

            var model = new InternalMoneyTransferCreateBindingModel
            {
                UserAccounts = userAccounts
            };

            return this.View(model);
        }

        [HttpPost]
        [EnsureOwnership]
        public async Task<IActionResult> Create(InternalMoneyTransferCreateBindingModel model)
        {
            var userId = await this.userService.GetUserIdByUsernameAsync(this.User.Identity.Name);

            if (!this.ModelState.IsValid)
            {
                model.UserAccounts = await this.GetAllUserAccountsAsync(userId);
                return this.View(model);
            }

            var account =
                await this.bankAccountService.GetBankAccountAsync<BankAccountIndexServiceModel>(model.AccountId);
            if (account.Balance < model.Amount)
            {
                this.ShowErrorMessage(NotificationMessages.InsufficientFunds);
                model.UserAccounts = await this.GetAllUserAccountsAsync(userId);
                return this.View(model);
            }

            var destinationAccountId =
                await this.bankAccountService.GetAccountIdAsync(model.DestinationBankAccountUniqueId);

            if (destinationAccountId == null)
            {
                this.ShowErrorMessage(NotificationMessages.DestinationBankAccountDoesNotExist);
                model.UserAccounts = await this.GetAllUserAccountsAsync(userId);
                return this.View(model);
            }

            var sourceServiceModel = Mapper.Map<MoneyTransferCreateServiceModel>(model);
            sourceServiceModel.Source = account.UniqueId;
            sourceServiceModel.Amount = -sourceServiceModel.Amount;

            if (!await this.moneyTransferService.CreateMoneyTransferAsync(sourceServiceModel))
            {
                this.ShowErrorMessage(NotificationMessages.TryAgainLaterError);
                model.UserAccounts = await this.GetAllUserAccountsAsync(userId);
                return this.View(model);
            }

            var destinationServiceModel = Mapper.Map<MoneyTransferCreateServiceModel>(model);
            destinationServiceModel.Source = account.UniqueId;
            destinationServiceModel.AccountId = destinationAccountId;

            if (!await this.moneyTransferService.CreateMoneyTransferAsync(destinationServiceModel))
            {
                this.ShowErrorMessage(NotificationMessages.TryAgainLaterError);
                model.UserAccounts = await this.GetAllUserAccountsAsync(userId);
                return this.View(model);
            }

            this.ShowSuccessMessage(NotificationMessages.SuccessfulMoneyTransfer);
            return this.RedirectToHome();
        }

        private async Task<SelectListItem[]> GetAllUserAccountsAsync(string userId)
        {
            var userAccounts = await this.bankAccountService
                .GetAllUserAccountsAsync<BankAccountIndexServiceModel>(userId);

            return userAccounts
                .Select(a => new SelectListItem {Text = $"{a.Name} - ({a.Balance} EUR)", Value = a.Id})
                .ToArray();
        }
    }
}