namespace BankSystem.Web.Areas.MoneyTransfers.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Services.Interfaces;
    using Services.Models.MoneyTransfer;

    public class HomeController : BaseMoneyTransferController
    {
        private readonly IMoneyTransferService moneyTransferService;
        private readonly IUserService userService;

        public HomeController(IBankAccountService bankAccountService, IMoneyTransferService moneyTransferService, IUserService userService)
            : base(bankAccountService)
        {
            this.moneyTransferService = moneyTransferService;
            this.userService = userService;
        }

        [Route("/{area}/Archives")]
        public async Task<IActionResult> All()
        {
            var userId = await this.userService.GetUserIdByUsernameAsync(this.User.Identity.Name);
            var transfers = (await this.moneyTransferService.GetAllMoneyTransfersAsync<MoneyTransferListingServiceModel>(userId))
                .Select(Mapper.Map<MoneyTransferListingViewModel>)
                .ToArray();

            return this.View(transfers);
        }
    }
}
