namespace BankSystem.Web.Api
{
    using System.Threading.Tasks;
    using Infrastructure.Filters;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Services.Interfaces;
    using Services.Models.MoneyTransfer;

    [Route("api/[controller]")]
    [ApiController]
    [EnsureRequest]
    public class ReceiveMoneyTransfersController : ControllerBase
    {
        private readonly IMoneyTransferService moneyTransferService;
        private readonly IBankAccountService bankAccountService;

        public ReceiveMoneyTransfersController(IMoneyTransferService moneyTransferService, IBankAccountService bankAccountService)
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

            var accountId = await this.bankAccountService.GetAccountIdAsync(model.DestinationBankAccountUniqueId);
            if (accountId == null)
            {
                return this.NoContent();
            }

            var serviceModel = new MoneyTransferCreateServiceModel
            {
                AccountId = accountId,
                Amount = model.Amount,
                Description = model.Description,
                DestinationBankAccountUniqueId = model.DestinationBankAccountUniqueId,
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