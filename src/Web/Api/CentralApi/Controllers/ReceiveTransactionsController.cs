namespace CentralApi.Controllers
{
    using System.Net.Http;
    using System.Threading.Tasks;
    using AutoMapper;
    using Infrastructure.Filters;
    using Infrastructure.Handlers;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Models;
    using Services.Interfaces;
    using Services.Models.Banks;

    [Route("api/[controller]")]
    [ApiController]
    [EnsureRequestIsValid]
    public class ReceiveTransactionsController : ControllerBase
    {
        private readonly IBanksService banksService;
        private readonly IConfiguration configuration;

        public ReceiveTransactionsController(IBanksService banksService, IConfiguration configuration)
        {
            this.banksService = banksService;
            this.configuration = configuration;
        }

        // POST api/values
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ReceiveTransactionModel model)
        {
            if (!this.TryValidateModel(model))
            {
                return this.NoContent();
            }

            var bank = await this.banksService.GetBankAsync<BankServiceModel>(model.DestinationBankName, model.DestinationBankSwiftCode,
                model.DestinationBankCountry);

            if (bank == null)
            {
                return this.NotFound($"{model.DestinationBankName} was not found.");
            }

            var customHandler = new CustomCentralApiDelegatingHandler(this.configuration.GetSection("CentralApiConfiguration:Key").Value, bank.ApiKey);
            var client = HttpClientFactory.Create(customHandler);
            var sendModel = Mapper.Map<SendTransactionModel>(model);
            var response = await client.PostAsJsonAsync(bank.ApiAddress, sendModel);
            if (!response.IsSuccessStatusCode)
            {
                return this.NotFound();
            }

            return this.Ok();
        }
    }
}
