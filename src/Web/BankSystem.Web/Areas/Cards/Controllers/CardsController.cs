namespace BankSystem.Web.Areas.Cards.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using Common;
    using Infrastructure.Extensions;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Models;
    using Services.Interfaces;
    using Services.Models.BankAccount;
    using Services.Models.Card;

    public class CardsController : BaseCardsController
    {
        private const int CardsCountPerPage = 10;
        private readonly IBankAccountService bankAccountService;
        private readonly ICardService cardService;

        private readonly IUserService userService;

        public CardsController(
            IUserService userService, 
            IBankAccountService bankAccountService,
            ICardService cardService)
        {
            this.userService = userService;
            this.bankAccountService = bankAccountService;
            this.cardService = cardService;
        }

        public async Task<IActionResult> Index(int pageIndex = 1)
        {
            var userId = await this.userService.GetUserIdByUsernameAsync(this.User.Identity.Name);
            var allCards = (await this.cardService
                    .GetAllCardsAsync<CardDetailsServiceModel>(userId))
                .Select(Mapper.Map<CardListingDto>)
                .ToPaginatedList(pageIndex, CardsCountPerPage);

            var cards = new CardListingViewModel
            {
                Cards = allCards
            };

            return this.View(cards);
        }

        public async Task<IActionResult> Create()
        {
            var userId = await this.userService.GetUserIdByUsernameAsync(this.User.Identity.Name);
            var userAccounts = await this.GetAllAccountsAsync(userId);

            var model = new CardCreateViewModel
            {
                BankAccounts = userAccounts
            };

            return this.View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CardCreateViewModel model)
        {
            var userId = await this.userService.GetUserIdByUsernameAsync(this.User.Identity.Name);
            if (!this.ModelState.IsValid)
            {
                model.BankAccounts = await this.GetAllAccountsAsync(userId);
                return this.View(model);
            }
            
            var account = await this.bankAccountService.GetByIdAsync<BankAccountDetailsServiceModel>(model.AccountId);
            if (account == null ||
                account.UserUserName != this.User.Identity.Name)
            {
                return this.Forbid();
            }

            var serviceModel = Mapper.Map<CardCreateServiceModel>(model);
            serviceModel.UserId = userId;
            serviceModel.Name = account.UserFullName;
            serviceModel.ExpiryDate = DateTime.UtcNow.AddYears(GlobalConstants.CardValidityInYears)
                .ToString(GlobalConstants.CardExpirationDateFormat);

            var isCreated = await this.cardService.CreateAsync(serviceModel);
            if (!isCreated)
            {
                this.ShowErrorMessage(NotificationMessages.CardCreateError);
                return this.RedirectToHome();
            }

            this.ShowSuccessMessage(NotificationMessages.CardCreatedSuccessfully);
            return this.RedirectToAction(nameof(this.Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                this.ShowErrorMessage(NotificationMessages.CardDoesNotExist);
                return this.RedirectToAction(nameof(this.Index));
            }

            var card = await this.cardService.GetAsync<CardDetailsServiceModel>(id);

            var userId = await this.userService.GetUserIdByUsernameAsync(this.User.Identity.Name);

            if (card == null || card.UserId != userId)
            {
                this.ShowErrorMessage(NotificationMessages.CardDoesNotExist);
                return this.RedirectToAction(nameof(this.Index));
            }

            var isDeleted = await this.cardService.DeleteAsync(id);
            if (!isDeleted)
            {
                this.ShowErrorMessage(NotificationMessages.CardDeleteError);
            }
            else
            {
                this.ShowSuccessMessage(NotificationMessages.CardDeletedSuccessfully);
            }

            return this.RedirectToAction(nameof(this.Index));
        }

        private async Task<IEnumerable<SelectListItem>> GetAllAccountsAsync(string userId)
            => (await this.bankAccountService
                    .GetAllAccountsByUserIdAsync<BankAccountIndexServiceModel>(userId))
                .Select(a => new SelectListItem {Text = a.Name, Value = a.Id})
                .ToArray();
    }
}