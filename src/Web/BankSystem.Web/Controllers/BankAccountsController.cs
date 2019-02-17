namespace BankSystem.Web.Controllers
{
    using System;
    using System.Threading.Tasks;
    using AutoMapper;
    using Common;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Models.BankAccount;
    using Services.Interfaces;
    using Services.Models.BankAccount;

    [Authorize]
    public class BankAccountsController : BaseController
    {
        private readonly IBankAccountService bankAccountService;
        private readonly IUserService userService;

        public BankAccountsController(IBankAccountService bankAccountService, IUserService userService)
        {
            this.bankAccountService = bankAccountService;
            this.userService = userService;
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
                this.Error(NotificationMessages.BankAccountCreateError);

                return this.View(model);
            }

            this.Success(NotificationMessages.BankAccountCreated);

            // TODO Redirect to account details page
            return this.RedirectToAction("Index", "Home");
        }
    }
}