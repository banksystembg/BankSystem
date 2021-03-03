namespace BankSystem.Web.Areas.MoneyTransfers.Controllers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using Common;
    using Infrastructure.Helpers.GlobalTransferHelpers;
    using Infrastructure.Helpers.GlobalTransferHelpers.Models;
    using Microsoft.AspNetCore.Mvc;
    using Models.Global.Create;
    using Services.BankAccount;
    using Services.Models.BankAccount;
    using Services.User;

    public class GlobalController : BaseMoneyTransferController
    {
        private readonly IBankAccountService bankAccountService;
        private readonly IGlobalTransferHelper globalTransferHelper;
        private readonly IUserService userService;

        public GlobalController(
            IBankAccountService bankAccountService,
            IUserService userService,
            IGlobalTransferHelper globalTransferHelper,
            IMapper mapper)
            : base(bankAccountService, mapper)
        {
            this.bankAccountService = bankAccountService;
            this.userService = userService;
            this.globalTransferHelper = globalTransferHelper;
        }

        public async Task<IActionResult> Create()
        {
            var userId = this.GetCurrentUserId();
            var userAccounts = await this.GetAllAccountsAsync(userId);
            if (!userAccounts.Any())
            {
                this.ShowErrorMessage(NotificationMessages.NoAccountsError);

                return this.RedirectToHome();
            }

            var model = new GlobalMoneyTransferCreateBindingModel
            {
                OwnAccounts = userAccounts,
                SenderName = await this.userService.GetAccountOwnerFullnameAsync(userId)
            };

            return this.View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(GlobalMoneyTransferCreateBindingModel model)
        {
            var userId = this.GetCurrentUserId();
            model.SenderName = await this.userService.GetAccountOwnerFullnameAsync(userId);
            if (!this.TryValidateModel(model))
            {
                model.OwnAccounts = await this.GetAllAccountsAsync(userId);

                return this.View(model);
            }

            var account =
                await this.bankAccountService.GetByIdAsync<BankAccountDetailsServiceModel>(model.AccountId);
            if (account == null || account.UserId != userId)
            {
                return this.Forbid();
            }

            if (string.Equals(account.UniqueId, model.DestinationBank.Account.UniqueId,
                StringComparison.InvariantCulture))
            {
                this.ShowErrorMessage(NotificationMessages.SameAccountsError);
                model.OwnAccounts = await this.GetAllAccountsAsync(userId);

                return this.View(model);
            }

            var serviceModel = this.Mapper.Map<GlobalTransferDto>(model);
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