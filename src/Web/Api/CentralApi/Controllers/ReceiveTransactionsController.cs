namespace CentralApi.Controllers
{
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using AutoMapper;
    using Infrastructure.Filters;
    using Infrastructure.Helpers;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using Models;
    using Newtonsoft.Json;
    using Services.Bank;
    using Services.Models.Banks;

    [ApiController]
    [Route("api/[controller]")]
    [DecryptAndVerifyRequest]
    public class ReceiveTransactionsController : ControllerBase
    {
        private const string BankRefusedTheRequestMessage =
            "{0} refused the transfer. If this error continues to occur please contact our support center";

        private const string BankNotFound = "{0} was not found.";

        private readonly IBanksService banksService;
        private readonly IMapper mapper;
        private readonly CentralApiConfiguration configuration;

        public ReceiveTransactionsController(
            IBanksService banksService,
            IMapper mapper,
            IOptions<CentralApiConfiguration> configuration)
        {
            this.banksService = banksService;
            this.mapper = mapper;
            this.configuration = configuration.Value;
        }

        // POST api/values
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] string data)
        {
            var model = JsonConvert.DeserializeObject<ReceiveTransactionModel>(data);
            var bank = await this.banksService.GetBankAsync<BankServiceModel>(model.DestinationBankName,
                model.DestinationBankSwiftCode,
                model.DestinationBankCountry);

            if (bank == null)
            {
                return this.NotFound(string.Format(BankNotFound, model.DestinationBankName));
            }

            var sendModel = this.mapper.Map<SendTransactionModel>(model);
            var encryptedAndSignedData =
                TransactionHelper.SignAndEncryptData(sendModel, this.configuration.Key, bank.ApiKey);
            var client = new HttpClient();
            var response = await client.PostAsJsonAsync(bank.ApiAddress, encryptedAndSignedData);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                return this.BadRequest(string.Format(BankRefusedTheRequestMessage, model.DestinationBankName));
            }

            return this.Ok();
        }
    }
}