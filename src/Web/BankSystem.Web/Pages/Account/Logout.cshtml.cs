namespace BankSystem.Web.Pages.Account
{
    using BankSystem.Models;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using System.Threading.Tasks;

    [AllowAnonymous]
    public class LogoutModel : BasePageModel
    {
        private readonly ILogger<LogoutModel> logger;
        private readonly SignInManager<BankUser> signInManager;

        public LogoutModel(SignInManager<BankUser> signInManager, ILogger<LogoutModel> logger)
        {
            this.signInManager = signInManager;
            this.logger = logger;
        }

        public IActionResult OnGet()
            => !this.User.Identity.IsAuthenticated ? this.RedirectToHome() : this.Page();

        public async Task<IActionResult> OnPost()
        {
            if (!this.User.Identity.IsAuthenticated)
            {
                return this.RedirectToHome();
            }

            await this.signInManager.SignOutAsync();
            this.logger.LogInformation("User logged out.");

            return this.RedirectToHome();
        }
    }
}