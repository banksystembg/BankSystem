namespace BankSystem.Web.Api
{
    using Areas.MoneyTransfers.Models.Global;
    using AutoMapper;
    using Common;
    using Common.Utils.CustomHandlers;
    using Infrastructure.Filters;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Services.Interfaces;
    using Services.Models.BankAccount;
    using Services.Models.MoneyTransfer;
    using System.Net.Http;
    using System.Threading.Tasks;

    [Route("api/[controller]")]
    [IgnoreAntiforgeryToken]
    [EnsureRequest]
    [ApiController]
    public class CardPaymentsController : ControllerBase
    {
        private readonly IBankConfigurationHelper bankConfigurationHelper;
        private readonly IBankAccountService bankAccountService;
        private readonly IMoneyTransferService moneyTransferService;

        public CardPaymentsController(
            IBankConfigurationHelper bankConfigurationHelper,
            IBankAccountService bankAccountService,
            IMoneyTransferService moneyTransferService)
        {
            this.bankConfigurationHelper = bankConfigurationHelper;
            this.bankAccountService = bankAccountService;
            this.moneyTransferService = moneyTransferService;
        }

        // POST: api/CardPayments
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] PaymentInfoModel model)
        {
            if (model.Amount <= 0)
            {
                return this.BadRequest();
            }

            var accountId = await this.bankAccountService.GetAccountIdAsync(model.Number, model.ParsedExpiryDate, model.SecurityCode,
                model.Name);
            // transfer money to destination account
            var error = await this.ExecuteTransfer(model, accountId);

            return error ?? this.Ok();
        }

        private async Task<IActionResult> ExecuteTransfer(PaymentInfoModel paymentInfo, string sourceAccountId)
        {
            var account = await this.bankAccountService
                .GetBankAccountAsync<BankAccountIndexServiceModel>(sourceAccountId);

            var transferModel = new GlobalMoneyTransferCentralApiBindingModel
            {
                Amount = paymentInfo.Amount,
                Description = paymentInfo.Description,
                DestinationBankName = paymentInfo.DestinationBankName,
                DestinationBankCountry = paymentInfo.DestinationBankCountry,
                DestinationBankSwiftCode = paymentInfo.DestinationBankSwiftCode,
                DestinationBankAccountUniqueId = paymentInfo.DestinationBankAccountUniqueId,
                SenderName = account.UserFullName,
                SenderAccountUniqueId = account.UniqueId
            };

            // verify there is enough money in the account
            if (account.Balance < transferModel.Amount)
            {
                return this.BadRequest();
            }

            // contact central api
            var response = await this.ContactCentralApiAsync(transferModel);
            if (!response.IsSuccessStatusCode)
            {
                return this.BadRequest();
            }

            // remove money from source account
            var serviceModel = Mapper.Map<MoneyTransferCreateServiceModel>(transferModel);
            serviceModel.Source = account.UniqueId;
            serviceModel.AccountId = sourceAccountId;
            serviceModel.Amount *= -1;

            var success = await this.moneyTransferService.CreateMoneyTransferAsync(serviceModel);
            return !success ? this.BadRequest() : null;
        }

        private async Task<HttpResponseMessage> ContactCentralApiAsync(GlobalMoneyTransferCentralApiBindingModel model)
        {
            var customHandler = new CustomDelegatingHandler(this.bankConfigurationHelper.CentralApiPublicKey,
                this.bankConfigurationHelper.Key,
                WebConstants.BankName, this.bankConfigurationHelper.UniqueIdentifier);
            var client = HttpClientFactory.Create(customHandler);

            return await client.PostAsJsonAsync($"{GlobalConstants.CentralApiBaseAddress}api/ReceiveTransactions",
                model);
        }
    }
}
