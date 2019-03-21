namespace BankSystem.Web.Api
{
    using AutoMapper;
    using Infrastructure.Filters;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Services.Interfaces;
    using System.Threading.Tasks;
    using Services.Models.GlobalTransfer;

    [Route("api/[controller]")]
    [IgnoreAntiforgeryToken]
    [EnsureRequest]
    [ApiController]
    public class CardPaymentsController : ControllerBase
    {
        private readonly IGlobalTransferHelper globalTransferHelper;
        private readonly IBankAccountService bankAccountService;

        public CardPaymentsController(
            IGlobalTransferHelper globalTransferHelper,
            IBankAccountService bankAccountService)
        {
            this.globalTransferHelper = globalTransferHelper;
            this.bankAccountService = bankAccountService;
        }

        // POST: api/CardPayments
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] PaymentInfoModel model)
        {
            if (model.Amount <= 0)
            {
                return this.BadRequest();
            }

            var accountId = await this.bankAccountService.GetAccountIdAsync(
                model.Number,
                model.ParsedExpiryDate,
                model.SecurityCode,
                model.Name);

            var serviceModel = Mapper.Map<GlobalTransferServiceModel>(model);
            serviceModel.SourceAccountId = accountId;

            var result = await this.globalTransferHelper.TransferMoneyAsync(serviceModel);

            if (result != GlobalTransferResult.Succeeded)
            {
                return this.BadRequest();
            }

            return this.Ok();
        }
    }
}