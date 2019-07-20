namespace BankSystem.Web.Areas.MoneyTransfers.Controllers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using Infrastructure.Extensions;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Services.Interfaces;
    using Services.Models.MoneyTransfer;

    public class HomeController : BaseMoneyTransferController
    {
        private const int PaymentsCountPerPage = 10;

        private readonly IMoneyTransferService moneyTransferService;
        private readonly IUserService userService;

        public HomeController(
            IBankAccountService bankAccountService,
            IMoneyTransferService moneyTransferService,
            IUserService userService)
            : base(bankAccountService)
        {
            this.moneyTransferService = moneyTransferService;
            this.userService = userService;
        }

        [Route("/{area}/Archives")]
        public async Task<IActionResult> All(int pageIndex = 1)
        {
            pageIndex = Math.Max(1, pageIndex);

            var userId = await this.userService.GetUserIdByUsernameAsync(this.User.Identity.Name);
            var allMoneyTransfers =
                (await this.moneyTransferService.GetMoneyTransfersAsync<MoneyTransferListingServiceModel>(userId, pageIndex, PaymentsCountPerPage))
                .Select(Mapper.Map<MoneyTransferListingDto>)
                .ToPaginatedList(await this.moneyTransferService.GetCountOfAllMoneyTransfersForUserAsync(userId), pageIndex, PaymentsCountPerPage);

            var transfers = new MoneyTransferListingViewModel
            {
                MoneyTransfers = allMoneyTransfers
            };

            return this.View(transfers);
        }
    }
}