namespace BankSystem.Web.Areas.Administration.Controllers
{
    using AutoMapper;
    using Microsoft.AspNetCore.Mvc;
    using MoneyTransfers.Models;
    using Services.Interfaces;
    using Services.Models.MoneyTransfer;
    using System.Linq;
    using System.Threading.Tasks;
    using Models;

    public class TransactionsController : BaseAdministrationController
    {
        private readonly IMoneyTransferService moneyTransfer;

        public TransactionsController(IMoneyTransferService moneyTransfer)
        {
            this.moneyTransfer = moneyTransfer;
        }

        public IActionResult Search() => this.View();

        public async Task<IActionResult> Result(string referenceNumber)
        {
            if (referenceNumber == null)
            {
                return this.View();
            }

            var moneyTransfers = (await this.moneyTransfer
                .GetMoneyTransferAsync<MoneyTransferListingServiceModel>(referenceNumber))
                .Select(Mapper.Map<MoneyTransferListingDto>);

            var viewModel = new TransactionListingViewModel
            {
                MoneyTransfers = moneyTransfers,
                ReferenceNumber = referenceNumber
            };

            return this.View(viewModel);
        }
    }
}
