namespace BankSystem.Web.Areas.MoneyTransfers.Controllers
{
    using AutoMapper;
    using Common;
    using Infrastructure.Filters;
    using Microsoft.AspNetCore.Mvc;
    using Models.Internal;
    using Services.Interfaces;
    using Services.Models.BankAccount;
    using Services.Models.MoneyTransfer;
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    public class InternalController : BaseMoneyTransferController
    {
        private readonly IMoneyTransferService moneyTransferService;
        private readonly IBankAccountService bankAccountService;
        private readonly IUserService userService;

        public InternalController(
            IMoneyTransferService moneyTransferService,
            IBankAccountService bankAccountService,
            IUserService userService)
            : base(bankAccountService)
        {
            this.moneyTransferService = moneyTransferService;
            this.userService = userService;
            this.bankAccountService = bankAccountService;
        }

        public async Task<IActionResult> Create()
        {
            var userId = await this.userService.GetUserIdByUsernameAsync(this.User.Identity.Name);
            var userAccounts = await this.GetAllAccountsAsync(userId);

            if (!userAccounts.Any())
            {
                this.ShowErrorMessage(NotificationMessages.NoAccountsError);
                return this.RedirectToHome();
            }

            var model = new InternalMoneyTransferCreateBindingModel
            {
                OwnAccounts = userAccounts
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
                model.OwnAccounts = await this.GetAllAccountsAsync(userId);
                return this.View(model);
            }

            var account =
                await this.bankAccountService.GetBankAccountAsync<BankAccountIndexServiceModel>(model.AccountId);
            if (string.Equals(account.UniqueId, model.DestinationBankAccountUniqueId, StringComparison.InvariantCulture))
            {
                this.ShowErrorMessage(NotificationMessages.SameAccountsError);
                model.OwnAccounts = await this.GetAllAccountsAsync(userId);
                return this.View(model);
            }
            if (account.Balance < model.Amount)
            {
                this.ShowErrorMessage(NotificationMessages.InsufficientFunds);
                model.OwnAccounts = await this.GetAllAccountsAsync(userId);
                return this.View(model);
            }

            var destinationAccountId = await this.bankAccountService.GetAccountIdAsync(model.DestinationBankAccountUniqueId);
            if (destinationAccountId == null)
            {
                this.ShowErrorMessage(NotificationMessages.DestinationBankAccountDoesNotExist);
                model.OwnAccounts = await this.GetAllAccountsAsync(userId);
                return this.View(model);
            }

            var sourceServiceModel = Mapper.Map<MoneyTransferCreateServiceModel>(model);
            sourceServiceModel.Source = account.UniqueId;
            sourceServiceModel.Amount = -sourceServiceModel.Amount;
            sourceServiceModel.SenderName = account.UserFullName;
            sourceServiceModel.RecipientName = account.UserFullName;

            if (!await this.moneyTransferService.CreateMoneyTransferAsync(sourceServiceModel))
            {
                this.ShowErrorMessage(NotificationMessages.TryAgainLaterError);
                model.OwnAccounts = await this.GetAllAccountsAsync(userId);
                return this.View(model);
            }

            var destinationBankAccountUserFullName =
                await this.bankAccountService.GetBankAccountUserFullNameAsync(destinationAccountId);
            var destinationServiceModel = Mapper.Map<MoneyTransferCreateServiceModel>(model);
            destinationServiceModel.Source = account.UniqueId;
            destinationServiceModel.AccountId = destinationAccountId;
            destinationServiceModel.SenderName = account.UserFullName;
            destinationServiceModel.RecipientName = destinationBankAccountUserFullName;

            if (!await this.moneyTransferService.CreateMoneyTransferAsync(destinationServiceModel))
            {
                this.ShowErrorMessage(NotificationMessages.TryAgainLaterError);
                model.OwnAccounts = await this.GetAllAccountsAsync(userId);
                return this.View(model);
            }

            this.ShowSuccessMessage(NotificationMessages.SuccessfulMoneyTransfer);
            return this.RedirectToHome();
        }
    }
}