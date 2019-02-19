namespace CentralApi.Controllers
{
    using Infrastructure.Filters;
    using Microsoft.AspNetCore.Mvc;
    using Models;

    [Route("api/[controller]")]
    [ApiController]
    [EnsureRequestIsValid]
    public class ReceiveWorldwideTransactionsController : ControllerBase
    {
        // POST api/values
        [HttpPost]
        public void Post([FromBody] ReceiveTransactionModel model)
        {
        }
    }
}
