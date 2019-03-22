namespace BankSystem.Web.Api
{
    using System;
    using AutoMapper;
    using Infrastructure.Filters;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Services.Interfaces;
    using System.Threading.Tasks;
    using Services.Models.Card;
    using Services.Models.GlobalTransfer;

    [Route("api/[controller]")]
    [IgnoreAntiforgeryToken]
    [EnsureRequest]
    [ApiController]
    public class CardPaymentsController : ControllerBase
    {
        private readonly IGlobalTransferHelper globalTransferHelper;
        private readonly ICardService cardService;

        public CardPaymentsController(IGlobalTransferHelper globalTransferHelper, ICardService cardService)
        {
            this.globalTransferHelper = globalTransferHelper;
            this.cardService = cardService;
        }

        // POST: api/CardPayments
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] PaymentInfoModel model)
        {
            if (model.Amount <= 0)
            {
                return this.BadRequest();
            }

            var card = await this.cardService.GetAsync<CardDetailsServiceModel>(
                model.Number,
                model.ParsedExpiryDate,
                model.SecurityCode,
                model.Name);

            if (card == null || card.ExpiryDate < DateTime.UtcNow)
            {
                return this.BadRequest();
            }

            var serviceModel = Mapper.Map<GlobalTransferServiceModel>(model);
            serviceModel.SourceAccountId = card.AccountId;

            var result = await this.globalTransferHelper.TransferMoneyAsync(serviceModel);

            if (result != GlobalTransferResult.Succeeded)
            {
                return this.BadRequest();
            }

            return this.Ok();
        }
    }
}