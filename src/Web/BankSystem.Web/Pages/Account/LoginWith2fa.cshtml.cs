namespace BankSystem.Web.Pages.Account
{
    using System.ComponentModel.DataAnnotations;
    using System.Threading.Tasks;
    using BankSystem.Models;
    using Common;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    [AllowAnonymous]
    public class LoginWith2faModel : BasePageModel
    {
        private readonly ILogger<LoginWith2faModel> logger;
        private readonly SignInManager<BankUser> signInManager;

        public LoginWith2faModel(SignInManager<BankUser> signInManager, ILogger<LoginWith2faModel> logger)
        {
            this.signInManager = signInManager;
            this.logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public bool RememberMe { get; set; }

        public string ReturnUrl { get; set; }

        public async Task<IActionResult> OnGetAsync(bool rememberMe, string returnUrl = null)
        {
            returnUrl = returnUrl ?? this.Url.Content("~/");

            var user = await this.signInManager.GetTwoFactorAuthenticationUserAsync();

            if (user == null)
            {
                return this.LocalRedirect(returnUrl);
            }

            this.ReturnUrl = returnUrl;
            this.RememberMe = rememberMe;

            return this.Page();
        }

        public async Task<IActionResult> OnPostAsync(bool rememberMe, string returnUrl = null)
        {
            if (!this.ModelState.IsValid)
            {
                return this.Page();
            }

            returnUrl = returnUrl ?? this.Url.Content("~/");

            var user = await this.signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return this.LocalRedirect(returnUrl);
            }

            string authenticatorCode = this.Input.TwoFactorCode.Replace(" ", string.Empty)
                .Replace("-", string.Empty);

            var result =
                await this.signInManager.TwoFactorAuthenticatorSignInAsync(authenticatorCode, rememberMe,
                    this.Input.RememberMachine);

            if (!result.Succeeded)
            {
                if (result.IsLockedOut)
                {
                    await this.signInManager.SignOutAsync();

                    this.ShowErrorMessage(NotificationMessages.LoginLockedOut);
                    return this.RedirectToPage("./Login");
                }

                this.logger.LogWarning("Invalid authenticator code entered while logging in");

                this.ShowErrorMessage(NotificationMessages.TwoFactorAuthenticationCodeInvalid);
                return this.Page();
            }

            return this.LocalRedirect(returnUrl);
        }

        public class InputModel
        {
            [Required]
            [StringLength(7, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
                MinimumLength = 6)]
            [DataType(DataType.Text)]
            [Display(Name = "Authenticator code")]
            public string TwoFactorCode { get; set; }

            [Display(Name = "Remember this device")]
            public bool RememberMachine { get; set; }
        }
    }
}