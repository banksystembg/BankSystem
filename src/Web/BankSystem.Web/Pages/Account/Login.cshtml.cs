namespace BankSystem.Web.Pages.Account
{
    using BankSystem.Models;
    using Common;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using System.ComponentModel.DataAnnotations;
    using System.Text.Encodings.Web;
    using System.Threading.Tasks;

    [AllowAnonymous]
    public class LoginModel : BasePageModel
    {
        private const string EmailNotVerified =
            "Your email is not verified. Please verify your email by clicking <a href=\"{0}\">here</a>";

        private const string ResendEmailPage = "/Account/ReSendEmailVerification";

        private readonly ILogger<LoginModel> logger;
        private readonly SignInManager<BankUser> signInManager;

        public LoginModel(
            SignInManager<BankUser> signInManager,
            ILogger<LoginModel> logger)
        {
            this.signInManager = signInManager;
            this.logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public async Task<IActionResult> OnGetAsync(string returnUrl = null)
        {
            // Session has expired
            if (returnUrl != null)
            {
                this.ShowErrorMessage(NotificationMessages.SessionExpired);
            }

            returnUrl = returnUrl ?? this.Url.Content("~/");

            if (this.User.Identity.IsAuthenticated)
            {
                return this.LocalRedirect(returnUrl);
            }

            // Clear the existing external cookie to ensure a clean login process
            await this.HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            this.ReturnUrl = returnUrl;

            return this.Page();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? this.Url.Content("~/");

            if (this.User.Identity.IsAuthenticated)
            {
                return this.LocalRedirect(returnUrl);
            }

            if (!this.ModelState.IsValid)
            {
                return this.Page();
            }

            var result = await this.signInManager.PasswordSignInAsync(this.Input.Email, this.Input.Password,
                false, true);

            if (!result.Succeeded)
            {
                if (result.RequiresTwoFactor)
                {
                    return this.RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = false });
                }

                if (result.IsLockedOut)
                {
                    this.ShowErrorMessage(NotificationMessages.LoginLockedOut);
                    return this.Page();
                }


                if (result.IsNotAllowed)
                {
                    var callbackUrl = this.Url.Page(
                        ResendEmailPage,
                        null,
                        this.Request.Scheme);

                    this.TempData[GlobalConstants.TempDataEmailVerificationMessageKey] =
                        string.Format(EmailNotVerified, HtmlEncoder.Default.Encode(callbackUrl));
                    return this.Page();
                }

                this.ShowErrorMessage(NotificationMessages.InvalidCredentials);
                return this.Page();
            }

            var user = await this.signInManager.UserManager.FindByNameAsync(this.Input.Email);

            if (!user.TwoFactorEnabled && !this.Request.Cookies.ContainsKey(GlobalConstants.IgnoreTwoFactorWarningCookie))
            {
                this.TempData.Add(GlobalConstants.TempDataNoTwoFactorKey, true);
            }

            this.logger.LogInformation("User logged in.");
            return this.LocalRedirect(returnUrl);
        }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }
        }
    }
}