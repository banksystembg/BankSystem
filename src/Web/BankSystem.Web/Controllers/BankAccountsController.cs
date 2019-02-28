namespace BankSystem.Web.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;
    using Areas.MoneyTransfers.Models;
    using AutoMapper;
    using Common;
    using Microsoft.AspNetCore.Mvc;
    using Models.BankAccount;
    using Services.Interfaces;
    using Services.Models.BankAccount;
    using Services.Models.MoneyTransfer;

    public class BankAccountsController : BaseController
    {
        private readonly IBankAccountService bankAccountService;
        private readonly IUserService userService;
        private readonly IMoneyTransferService moneyTransferService;

        public BankAccountsController(IBankAccountService bankAccountService, IUserService userService, IMoneyTransferService moneyTransferService)
        {
            this.bankAccountService = bankAccountService;
            this.userService = userService;
            this.moneyTransferService = moneyTransferService;
        }

        public IActionResult Create()
        {
            return this.View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(BankAccountCreateBindingModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            var serviceModel = Mapper.Map<BankAccountCreateServiceModel>(model);

            serviceModel.UserId = await this.userService.GetUserIdByUsernameAsync(this.User.Identity.Name);

            var accountId = await this.bankAccountService.CreateAsync(serviceModel);

            if (accountId == null)
            {
                this.ShowErrorMessage(NotificationMessages.BankAccountCreateError);

                return this.View(model);
            }

            this.ShowSuccessMessage(NotificationMessages.BankAccountCreated);

            // TODO Redirect to account details page
            return this.RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Details(string id)
        {
            var transfers = (await this.moneyTransferService
                    .GetAllMoneyTransfersForAccountAsync<MoneyTransferListingServiceModel>(id))
                .Select(Mapper.Map<MoneyTransferListingViewModel>)
                .ToArray();

            return this.View(transfers);
        }
    }
}