namespace BankSystem.Web.Areas.MoneyTransfers.Controllers
{
    using AutoMapper;
    using Common;
    using Infrastructure.Filters;
    using Microsoft.AspNetCore.Mvc;
    using Models.Global.Create;
    using Services.Interfaces;
    using Services.Models.BankAccount;
    using Services.Models.GlobalTransfer;
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    public class GlobalController : BaseMoneyTransferController
    {
        private readonly IBankAccountService bankAccountService;
        private readonly IUserService userService;
        private readonly IGlobalTransferHelper globalTransferHelper;

        public GlobalController(IBankAccountService bankAccountService,
            IUserService userService,
            IGlobalTransferHelper globalTransferHelper)
            : base(bankAccountService)
        {
            this.bankAccountService = bankAccountService;
            this.userService = userService;
            this.globalTransferHelper = globalTransferHelper;
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

            var model = new GlobalMoneyTransferCreateBindingModel
            {
                OwnAccounts = userAccounts,
                SenderName = await this.userService.GetAccountOwnerFullnameAsync(userId),
            };

            return this.View(model);
        }

        [HttpPost]
        [EnsureOwnership]
        public async Task<IActionResult> Create(GlobalMoneyTransferCreateBindingModel model)
        {
            var userId = await this.userService.GetUserIdByUsernameAsync(this.User.Identity.Name);
            model.SenderName = await this.userService.GetAccountOwnerFullnameAsync(userId);
            if (!this.TryValidateModel(model))
            {
                model.OwnAccounts = await this.GetAllAccountsAsync(userId);
                return this.View(model);
            }

            var account =
                await this.bankAccountService.GetByIdAsync<BankAccountIndexServiceModel>(model.AccountId);
            if (string.Equals(account.UniqueId, model.DestinationBank.Account.UniqueId,
                StringComparison.InvariantCulture))
            {
                this.ShowErrorMessage(NotificationMessages.SameAccountsError);
                model.OwnAccounts = await this.GetAllAccountsAsync(userId);
                return this.View(model);
            }

            var serviceModel = Mapper.Map<GlobalTransferServiceModel>(model);
            serviceModel.SourceAccountId = model.AccountId;
            serviceModel.RecipientName = model.DestinationBank.Account.UserFullName;

            var result = await this.globalTransferHelper.TransferMoneyAsync(serviceModel);

            if (result != GlobalTransferResult.Succeeded)
            {
                if (result == GlobalTransferResult.InsufficientFunds)
                {
                    this.ShowErrorMessage(NotificationMessages.InsufficientFunds);
                    model.OwnAccounts = await this.GetAllAccountsAsync(userId);
                    return this.View(model);
                }

                this.ShowErrorMessage(NotificationMessages.TryAgainLaterError);
                return this.RedirectToHome();
            }

            this.ShowSuccessMessage(NotificationMessages.SuccessfulMoneyTransfer);
            return this.RedirectToHome();
        }
    }
}