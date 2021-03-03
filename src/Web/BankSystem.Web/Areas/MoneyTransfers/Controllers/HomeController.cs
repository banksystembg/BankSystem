namespace BankSystem.Web.Areas.MoneyTransfers.Controllers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using Infrastructure.Extensions;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Services.BankAccount;
    using Services.Models.MoneyTransfer;
    using Services.MoneyTransfer;

    public class HomeController : BaseMoneyTransferController
    {
        private const int PaymentsCountPerPage = 10;

        private readonly IMoneyTransferService moneyTransferService;
        
        public HomeController(
            IBankAccountService bankAccountService,
            IMoneyTransferService moneyTransferService,
            IMapper mapper)
            : base(bankAccountService, mapper)
        {
            this.moneyTransferService = moneyTransferService;
        }

        [Route("/{area}/Archives")]
        public async Task<IActionResult> All(int pageIndex = 1)
        {
            pageIndex = Math.Max(1, pageIndex);

            var userId = this.GetCurrentUserId();
            var allMoneyTransfers =
                (await this.moneyTransferService.GetMoneyTransfersAsync<MoneyTransferListingServiceModel>(userId,
                    pageIndex, PaymentsCountPerPage))
                .Select(this.Mapper.Map<MoneyTransferListingDto>)
                .ToPaginatedList(await this.moneyTransferService.GetCountOfAllMoneyTransfersForUserAsync(userId),
                    pageIndex, PaymentsCountPerPage);

            var transfers = new MoneyTransferListingViewModel
            {
                MoneyTransfers = allMoneyTransfers
            };

            return this.View(transfers);
        }
    }
}