namespace BankSystem.Web.Pages.Account.Settings
{
    using System.ComponentModel.DataAnnotations;
    using System.Text;
    using System.Text.Encodings.Web;
    using System.Threading.Tasks;
    using BankSystem.Models;
    using Common;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    [Authorize]
    public class Enable2faModel : BasePageModel
    {
        private const string AuthenticatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";
        private readonly ILogger<Enable2faModel> logger;
        private readonly SignInManager<BankUser> signInManager;
        private readonly UrlEncoder urlEncoder;
        private readonly UserManager<BankUser> userManager;

        public Enable2faModel(UserManager<BankUser> userManager, ILogger<Enable2faModel> logger,
            UrlEncoder urlEncoder, SignInManager<BankUser> signInManager)
        {
            this.userManager = userManager;
            this.logger = logger;
            this.urlEncoder = urlEncoder;
            this.signInManager = signInManager;
        }

        public string SharedKey { get; set; }

        public string AuthenticatorUri { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await this.userManager.GetUserAsync(this.User);
            if (user == null)
            {
                return this.Forbid();
            }

            if (await this.userManager.GetTwoFactorEnabledAsync(user))
            {
                return this.RedirectToPage("./Index");
            }

            await this.LoadSharedKeyAndQrCodeUriAsync(user, true);

            return this.Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await this.userManager.GetUserAsync(this.User);
            if (user == null)
            {
                return this.Forbid();
            }

            if (await this.userManager.GetTwoFactorEnabledAsync(user))
            {
                return this.RedirectToPage("./Index");
            }

            if (!this.ModelState.IsValid)
            {
                await this.LoadSharedKeyAndQrCodeUriAsync(user);
                return this.Page();
            }

            var isPasswordCorrect = await this.userManager.CheckPasswordAsync(user, this.Input.Password);
            if (!isPasswordCorrect)
            {
                this.ShowErrorMessage(NotificationMessages.InvalidPassword);
                await this.LoadSharedKeyAndQrCodeUriAsync(user);
                return this.Page();
            }

            string verificationCode = this.Input.Code.Replace(" ", string.Empty)
                .Replace("-", string.Empty);

            bool isTokenValid = await this.userManager.VerifyTwoFactorTokenAsync(user,
                this.userManager.Options.Tokens.AuthenticatorTokenProvider, verificationCode);

            if (!isTokenValid)
            {
                this.ShowErrorMessage(NotificationMessages.TwoFactorAuthenticationCodeInvalid);
                await this.LoadSharedKeyAndQrCodeUriAsync(user);
                return this.Page();
            }

            await this.userManager.SetTwoFactorEnabledAsync(user, true);

            await this.signInManager.RefreshSignInAsync(user);

            this.logger.LogInformation("User has enabled 2FA with an authenticator app.");

            this.ShowSuccessMessage(NotificationMessages.TwoFactorAuthenticationEnabled);
            return this.RedirectToPage("./Index");
        }

        private async Task LoadSharedKeyAndQrCodeUriAsync(BankUser user, bool resetKey = false)
        {
            string unformattedKey;
            if (resetKey ||
                string.IsNullOrEmpty(unformattedKey = await this.userManager.GetAuthenticatorKeyAsync(user)))
            {
                await this.userManager.ResetAuthenticatorKeyAsync(user);
                unformattedKey = await this.userManager.GetAuthenticatorKeyAsync(user);

                await this.signInManager.RefreshSignInAsync(user);
            }

            this.SharedKey = this.FormatKey(unformattedKey);

            string email = await this.userManager.GetEmailAsync(user);
            this.AuthenticatorUri = this.GenerateQrCodeUri(email, unformattedKey);
        }

        private string FormatKey(string unformattedKey)
        {
            var result = new StringBuilder();
            var currentPosition = 0;
            while (currentPosition + 4 < unformattedKey.Length)
            {
                result.Append(unformattedKey.Substring(currentPosition, 4)).Append(" ");
                currentPosition += 4;
            }

            if (currentPosition < unformattedKey.Length)
            {
                result.Append(unformattedKey.Substring(currentPosition));
            }

            return result.ToString().ToLowerInvariant();
        }

        private string GenerateQrCodeUri(string email, string unformattedKey)
            => string.Format(
                AuthenticatorUriFormat,
                this.urlEncoder.Encode(nameof(BankSystem)),
                this.urlEncoder.Encode(email),
                unformattedKey);

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