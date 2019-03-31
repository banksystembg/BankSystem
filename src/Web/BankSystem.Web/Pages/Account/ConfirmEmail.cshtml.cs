namespace BankSystem.Web.Pages.Account
{
    using System;
    using System.Threading.Tasks;
    using BankSystem.Models;
    using Common;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    [AllowAnonymous]
    public class ConfirmEmail : BasePageModel
    {
        private readonly UserManager<BankUser> userManager;

        public ConfirmEmail(UserManager<BankUser> userManager)
        {
            this.userManager = userManager;
        }

        public async Task<IActionResult> OnGetAsync(string userId, string code)
        {
            if (userId == null || code == null)
            {
                this.ShowErrorMessage(NotificationMessages.TryAgainLaterError);
                return this.RedirectToHome();
            }

            var user = await this.userManager.FindByIdAsync(userId);
            if (user == null)
            {
                this.ShowErrorMessage(NotificationMessages.TryAgainLaterError);
                return this.RedirectToHome();
            }

            var result = await this.userManager.ConfirmEmailAsync(user, code);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException($"Error confirming email for user with ID '{userId}':");
            }

            this.ShowSuccessMessage("You've successfully activated your account. You can now log in.");
            return this.RedirectToHome();
        }
    }
}