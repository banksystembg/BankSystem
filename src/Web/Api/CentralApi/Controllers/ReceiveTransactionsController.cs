namespace CentralApi.Controllers
{
    using System.Net.Http;
    using System.Threading.Tasks;
    using AutoMapper;
    using Infrastructure.Filters;
    using Infrastructure.Handlers;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using Models;
    using Services.Interfaces;
    using Services.Models.Banks;

    [Route("api/[controller]")]
    [ApiController]
    [EnsureRequestIsValid]
    public class ReceiveTransactionsController : ControllerBase
    {
        private const string BankRefusedTheRequestMessage = "{0} refused the transfer. If this error continues to occur please contact our support center";
        private const string BankNotFound = "{0} was not found.";

        private readonly IBanksService banksService;
        private readonly CentralApiConfiguration configuration;

        public ReceiveTransactionsController(IBanksService banksService, IOptions<CentralApiConfiguration> configuration)
        {
            this.banksService = banksService;
            this.configuration = configuration.Value;
        }

        // POST api/values
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ReceiveTransactionModel model)
        {
            var bank = await this.banksService.GetBankAsync<BankServiceModel>(model.DestinationBankName, model.DestinationBankSwiftCode,
                model.DestinationBankCountry);

            if (bank == null)
            {
                return this.NotFound(string.Format(BankNotFound, model.DestinationBankName));
            }

            var customHandler = new CustomCentralApiDelegatingHandler(this.configuration.Key, bank.ApiKey);
            var client = HttpClientFactory.Create(customHandler);
            var sendModel = Mapper.Map<SendTransactionModel>(model);
            var response = await client.PostAsJsonAsync(bank.ApiAddress, sendModel);
            if (!response.IsSuccessStatusCode)
            {
                return this.BadRequest(string.Format(BankRefusedTheRequestMessage, model.DestinationBankName));
            }

            return this.Ok();
        }
    }
}
