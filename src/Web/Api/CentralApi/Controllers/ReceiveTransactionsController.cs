namespace CentralApi.Controllers
{
    using System.Threading.Tasks;
    using Infrastructure.Filters;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Services.Interfaces;

    [Route("api/[controller]")]
    [ApiController]
    [EnsureRequestIsValid]
    public class ReceiveTransactionsController : ControllerBase
    {
        private readonly IBanksService banksService;

        public ReceiveTransactionsController(IBanksService banksService)
        {
            this.banksService = banksService;
        }

        // POST api/values
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ReceiveTransactionModel model)
        {
            if (!this.TryValidateModel(model))
            {
                return this.NoContent();
            }

            bool exists = await this.banksService.GetBankAsync(model.DestinationBankName, model.DestinationBankSwiftCode,
                model.DestinationBankCountry);

            if (!exists)
            {
                return this.NotFound(model.DestinationBankName);
            }

            // TODO: Contact destination bank


            return this.Ok();
        }
    }
}
