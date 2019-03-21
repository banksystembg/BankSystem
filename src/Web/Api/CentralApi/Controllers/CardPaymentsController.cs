namespace CentralApi.Controllers
{
    using Infrastructure.Handlers;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using Models;
    using Services.Interfaces;
    using Services.Models.Banks;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;

    [ApiController]
    [Route("api/[controller]")]
    public class CardPaymentsController : ControllerBase
    {
        private readonly IBanksService banksService;
        private readonly CentralApiConfiguration configuration;

        public CardPaymentsController(IBanksService banksService, IOptions<CentralApiConfiguration> configuration)
        {
            this.banksService = banksService;
            this.configuration = configuration.Value;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CardPaymentDto model)
        {
            try
            {
                var first3Digits = model.Number.Substring(0, 3);
                var bank = await this.banksService
                    .GetBankByBankIdentificationCardNumbersAsync<BankPaymentServiceModel>(first3Digits);
                if (bank?.CardPaymentUrl == null)
                {
                    return this.BadRequest();
                }

                var customHandler = new CustomCentralApiDelegatingHandler(this.configuration.Key, bank.ApiKey);
                var client = HttpClientFactory.Create(customHandler);
                var request = await client.PostAsJsonAsync(bank.CardPaymentUrl, model);

                if (request.StatusCode != HttpStatusCode.OK)
                {
                    return this.BadRequest();
                }

                return this.Ok();
            }
            catch
            {
                return this.BadRequest();
            }
        }
    }
}
