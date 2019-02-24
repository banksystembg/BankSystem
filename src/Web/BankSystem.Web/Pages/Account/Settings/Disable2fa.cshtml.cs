namespace BankSystem.Web.Pages.Account.Settings
{
    using System.Threading.Tasks;
    using BankSystem.Models;
    using Common;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    [Authorize]
    public class Disable2faModel : BasePageModel
    {
        private readonly UserManager<BankUser> userManager;
        private readonly SignInManager<BankUser> signInManager;
        private readonly ILogger<Disable2faModel> logger;

        public Disable2faModel(UserManager<BankUser> userManager, ILogger<Disable2faModel> logger,
            SignInManager<BankUser> signInManager)
        {
            this.userManager = userManager;
            this.logger = logger;
            this.signInManager = signInManager;
        }

        public async Task<IActionResult> OnGet()
        {
            var user = await this.userManager.GetUserAsync(this.User);
            if (user == null)
            {
                return this.Forbid();
            }

            if (!await this.userManager.GetTwoFactorEnabledAsync(user))
            {
                return this.RedirectToPage("./Index");
            }

            return this.Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await this.userManager.GetUserAsync(this.User);
            if (user == null)
            {
                return this.Forbid();
            }

            if (!await this.userManager.GetTwoFactorEnabledAsync(user))
            {
                return this.RedirectToPage("./Index");
            }

            var disable2FaResult = await this.userManager.SetTwoFactorEnabledAsync(user, false);
            if (!disable2FaResult.Succeeded)
            {
                this.ShowErrorMessage(NotificationMessages.TwoFactorAuthenticationDisableError);
                return this.Page();
            }

            await this.userManager.ResetAuthenticatorKeyAsync(user);

            await this.signInManager.RefreshSignInAsync(user);

            this.logger.LogInformation("User has disabled 2fa.");

            this.ShowSuccessMessage(NotificationMessages.TwoFactorAuthenticationDisabled);
            return this.RedirectToPage("./Index");
        }
    }
}