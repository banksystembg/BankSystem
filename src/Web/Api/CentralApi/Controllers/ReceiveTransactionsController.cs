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
            bool exists = await this.banksService.CheckWhetherBankExistsAsync(model.DestinationBankName, model.DestinationBankSwiftCode,
                model.DestinationBankCountry);

            if (!exists)
            {
                return this.NotFound();
            }

            return this.Ok();
        }
    }
}
