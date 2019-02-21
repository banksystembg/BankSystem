namespace BankSystem.Web.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using AutoMapper;
    using Common;
    using Common.Utils.CustomHandlers;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Models.MoneyTransfer;
    using Services.Interfaces;
    using Services.Models.BankAccount;
    using Services.Models.MoneyTransfer;

    [Authorize]
    public class MoneyTransfersController : BaseController
    {
        private readonly IBankConfigurationService bankConfigurationHelper;
        private readonly IMoneyTransferService moneyTransferService;
        private readonly IBankAccountService bankAccountService;
        private readonly IUserService userService;

        public MoneyTransfersController(
            IMoneyTransferService moneyTransferService,
            IBankAccountService bankAccountService,
            IUserService userService,
            IBankConfigurationService bankConfigurationHelper)
        {
            this.moneyTransferService = moneyTransferService;
            this.bankAccountService = bankAccountService;
            this.userService = userService;
            this.bankConfigurationHelper = bankConfigurationHelper;
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

            var model = new MoneyTransferCreateBindingModel
            {
                UserAccounts = userAccounts,
                SenderName = await this.userService.GetUserFullnameAsync(userId),
            };

            return this.View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(MoneyTransferCreateBindingModel model)
        {
            var userId = await this.userService.GetUserIdByUsernameAsync(this.User.Identity.Name);
            if (!this.TryValidateModel(model))
            {
                model.UserAccounts = await this.GetAllUserAccountsAsync(userId);
                model.SenderName = await this.userService.GetUserFullnameAsync(userId);
                return this.View(model);
            }

            // Check whether user have sufficient balance to make the payment
            var account = await this.bankAccountService.GetUserAccountBalanceAsync(model.AccountId);
            if (account < model.Amount)
            {
                this.ShowErrorMessage(NotificationMessages.InsufficientFunds);
                model.UserAccounts = await this.GetAllUserAccountsAsync(userId);
                model.SenderName = await this.userService.GetUserFullnameAsync(userId);
                return this.View(model);
            }

            // Contact central api
            var customHandler = new CustomDelegatingHandler(this.bankConfigurationHelper.CentralApiPublicKey, this.bankConfigurationHelper.Key,
                WebConstants.BankName, this.bankConfigurationHelper.UniqueIdentifier);
            var client = HttpClientFactory.Create(customHandler);

            var senderAccountUniqueId = await this.bankAccountService.GetUserAccountUniqueId(model.AccountId);
            var centralApiModel = Mapper.Map<MoneyTransferCentralApiBindingModel>(model);
            centralApiModel.SenderAccountUniqueId = senderAccountUniqueId;
            var response = await client.PostAsJsonAsync($"{GlobalConstants.CentralApiBaseAddress}api/ReceiveTransactions", centralApiModel);
            if (!response.IsSuccessStatusCode)
            {
                this.ShowErrorMessage(NotificationMessages.TryAgainLaterError);
                return this.RedirectToHome();
            }

            // If we got this far, the payment process is successful and we can store the data in database
            var serviceModel = Mapper.Map<MoneyTransferCreateServiceModel>(model);
            var isSuccessful = await this.moneyTransferService.CreateMoneyTransferAsync(serviceModel, true);
            if (!isSuccessful)
            {
                this.ShowErrorMessage(NotificationMessages.TryAgainLaterError);
            }

            this.ShowSuccessMessage(NotificationMessages.SuccessfulMoneyTransfer);
            return this.RedirectToHome();
        }

        private async Task<IEnumerable<SelectListItem>> GetAllUserAccountsAsync(string userId)
        {
            var userAccounts = await this.bankAccountService
                .GetAllUserAccountsAsync<BankAccountIndexServiceModel>(userId);

            return userAccounts
                .Select(a => new SelectListItem { Text = $"{a.Name} - ({a.Balance} EUR)", Value = a.Id })
                .ToArray();
        }
    }
}
