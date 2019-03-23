namespace BankSystem.Web.Pages.Account.Settings
{
    using System.ComponentModel.DataAnnotations;
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
        private readonly ILogger<Disable2faModel> logger;
        private readonly SignInManager<BankUser> signInManager;
        private readonly UserManager<BankUser> userManager;

        public Disable2faModel(UserManager<BankUser> userManager, ILogger<Disable2faModel> logger,
            SignInManager<BankUser> signInManager)
        {
            this.userManager = userManager;
            this.logger = logger;
            this.signInManager = signInManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

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

            if (!this.ModelState.IsValid)
            {
                return this.Page();
            }

            var isPasswordCorrect = await this.userManager.CheckPasswordAsync(user, this.Input.Password);
            if (!isPasswordCorrect)
            {
                this.ShowErrorMessage(NotificationMessages.InvalidPassword);
                return this.Page();
            }

            string verificationCode = this.Input.Code.Replace(" ", string.Empty)
                .Replace("-", string.Empty);

            bool isTokenValid = await this.userManager.VerifyTwoFactorTokenAsync(user,
                this.userManager.Options.Tokens.AuthenticatorTokenProvider, verificationCode);

            if (!isTokenValid)
            {
                this.ShowErrorMessage(NotificationMessages.TwoFactorAuthenticationCodeInvalid);
                return this.Page();
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

        public class InputModel
        {
            [Required]
            [StringLength(7, ErrorMessage = "The {0} must be 6 digits long",
                MinimumLength = 6)]
            [DataType(DataType.Text)]
            [Display(Name = "Verification Code")]
            public string Code { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }
        }
    }
}