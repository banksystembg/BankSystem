namespace BankSystem.Web.Pages.Account
{
    using BankSystem.Models;
    using Common;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;

    [AllowAnonymous]
    public class ConfirmEmail : BasePageModel
    {
        private readonly UserManager<BankUser> userManager;

        public ConfirmEmail(UserManager<BankUser> userManager)
            => this.userManager = userManager;

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
                this.ShowErrorMessage(NotificationMessages.AccountDoesNotExist);
                return this.RedirectToHome();
            }

            var result = await this.userManager.ConfirmEmailAsync(user, code);
            if (!result.Succeeded)
            {
                this.ShowErrorMessage(NotificationMessages.EmailVerificationFailed);
                return this.RedirectToHome();
            }

            this.ShowSuccessMessage(NotificationMessages.SuccessfulEmailVerification);
            return this.RedirectToLoginPage();
        }
    }
}