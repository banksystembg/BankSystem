namespace BankSystem.Web.Api
{
    using System;
    using System.Threading.Tasks;
    using Infrastructure.Filters;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Services.Interfaces;
    using Services.Models.BankAccount;
    using Services.Models.MoneyTransfer;

    [Route("api/[controller]")]
    [ApiController]
    [EnsureRequestIsSigned]
    [IgnoreAntiforgeryToken]
    public class ReceiveMoneyTransfersController : ControllerBase
    {
        private readonly IBankAccountService bankAccountService;
        private readonly IMoneyTransferService moneyTransferService;

        public ReceiveMoneyTransfersController(
            IMoneyTransferService moneyTransferService,
            IBankAccountService bankAccountService)
        {
            this.moneyTransferService = moneyTransferService;
            this.bankAccountService = bankAccountService;
        }

        // POST api/values
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ReceiveMoneyTransferModel model)
        {
            if (!this.TryValidateModel(model))
            {
                return this.NoContent();
            }

            var account =
                await this.bankAccountService.GetByUniqueIdAsync<BankAccountConciseServiceModel>(
                    model.DestinationBankAccountUniqueId);
            if (account == null || !string.Equals(account.UserFullName, model.RecipientName,
                    StringComparison.InvariantCulture))
            {
                return this.NoContent();
            }

            var serviceModel = new MoneyTransferCreateServiceModel
            {
                AccountId = account.Id,
                Amount = model.Amount,
                Description = model.Description,
                DestinationBankAccountUniqueId = model.DestinationBankAccountUniqueId,
                Source = model.SenderAccountUniqueId,
                SenderName = model.SenderName,
                RecipientName = model.RecipientName
            };

            var isSuccessful = await this.moneyTransferService.CreateMoneyTransferAsync(serviceModel);
            if (!isSuccessful)
            {
                return this.NoContent();
            }

            return this.Ok();
        }
    }
}