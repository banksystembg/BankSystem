namespace BankSystem.Web.Controllers.MoneyTransfers
{
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using AutoMapper;
    using Common;
    using Common.Utils.CustomHandlers;
    using Infrastructure.Filters;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Models.ForeignMoneyTransfers;
    using Models.ForeignMoneyTransfers.Create;
    using Services.Interfaces;
    using Services.Models.BankAccount;
    using Services.Models.MoneyTransfer;

    [Authorize]
    public class ForeignMoneyTransfersController : BaseMoneyTransferController
    {
        private readonly IBankConfigurationHelper bankConfigurationHelper;
        private readonly IMoneyTransferService moneyTransferService;
        private readonly IBankAccountService bankAccountService;
        private readonly IUserService userService;

        public ForeignMoneyTransfersController(
            IMoneyTransferService moneyTransferService,
            IBankAccountService bankAccountService,
            IUserService userService,
            IBankConfigurationHelper bankConfigurationHelper)
            : base(bankAccountService)
        {
            this.moneyTransferService = moneyTransferService;
            this.bankAccountService = bankAccountService;
            this.userService = userService;
            this.bankConfigurationHelper = bankConfigurationHelper;
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

            var model = new ForeignMoneyTransferCreateBindingModel
            {
                OwnAccounts = userAccounts,
                SenderName = await this.userService.GetAccountOwnerFullnameAsync(userId),
            };

            return this.View(model);
        }

        [HttpPost]
        [EnsureOwnership]
        public async Task<IActionResult> Create(ForeignMoneyTransferCreateBindingModel model)
        {
            var userId = await this.userService.GetUserIdByUsernameAsync(this.User.Identity.Name);
            model.SenderName = await this.userService.GetAccountOwnerFullnameAsync(userId);
            if (!this.TryValidateModel(model))
            {
                model.OwnAccounts = await this.GetAllAccountsAsync(userId);
                return this.View(model);
            }

            // Check whether user have sufficient balance to make the payment
            var account = await this.bankAccountService.GetBankAccountAsync<BankAccountIndexServiceModel>(model.AccountId);
            if (account.Balance < model.Amount)
            {
                this.ShowErrorMessage(NotificationMessages.InsufficientFunds);
                model.OwnAccounts = await this.GetAllAccountsAsync(userId);
                return this.View(model);
            }
            // Contact central api
            var response = await this.ContactCentralApiAsync(model);
            if (!response.IsSuccessStatusCode)
            {
                var dataStream = await response.Content.ReadAsStreamAsync();
                using (var reader = new StreamReader(dataStream))
                {
                    var strResponse = reader.ReadToEnd();

                    if (strResponse.Length > 300)
                    {
                        strResponse = NotificationMessages.TryAgainLaterError;
                    }

                    this.ShowErrorMessage(strResponse);
                    return this.RedirectToHome();
                }
            }

            // If we got this far, the payment process is successful and we can store the data in database
            var serviceModel = Mapper.Map<MoneyTransferCreateServiceModel>(model);
            serviceModel.Source = account.UniqueId;
            // Set amount to negative because user is sending money which have to be substracted from his acc balance
            serviceModel.Amount = model.Amount * (-1);

            var isSuccessful = await this.moneyTransferService.CreateMoneyTransferAsync(serviceModel);
            if (!isSuccessful)
            {
                this.ShowErrorMessage(NotificationMessages.TryAgainLaterError);
                return this.RedirectToHome();
            }

            this.ShowSuccessMessage(NotificationMessages.SuccessfulMoneyTransfer);
            return this.RedirectToHome();
        }

        private async Task<HttpResponseMessage> ContactCentralApiAsync(ForeignMoneyTransferCreateBindingModel model)
        {
            var customHandler = new CustomDelegatingHandler(this.bankConfigurationHelper.CentralApiPublicKey, this.bankConfigurationHelper.Key,
                WebConstants.BankName, this.bankConfigurationHelper.UniqueIdentifier);
            var client = HttpClientFactory.Create(customHandler);

            var senderAccountUniqueId = await this.bankAccountService.GetUserAccountUniqueId(model.AccountId);
            var centralApiModel = Mapper.Map<ForeignMoneyTransferCentralApiBindingModel>(model);
            centralApiModel.SenderAccountUniqueId = senderAccountUniqueId;
            return await client.PostAsJsonAsync($"{GlobalConstants.CentralApiBaseAddress}api/ReceiveTransactions", centralApiModel);
        }
    }
}
