namespace BankSystem.Web.Pages.Account
{
    using BankSystem.Models;
    using Common;
    using Common.EmailSender.Interface;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using System.ComponentModel.DataAnnotations;
    using System.Text.Encodings.Web;
    using System.Threading.Tasks;

    [AllowAnonymous]
    public class SendEmailVerificationModel : BasePageModel
    {
        private readonly UserManager<BankUser> userManager;
        private readonly SignInManager<BankUser> signInManager;
        private readonly IEmailSender emailSender;

        public SendEmailVerificationModel(
            UserManager<BankUser> userManager,
            SignInManager<BankUser> signInManager,
            IEmailSender emailSender)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        [BindProperty]
        public ReCaptchaModel Recaptcha { get; set; }

        public IActionResult OnGet()
            => this.Page();

        public async Task<IActionResult> OnPostAsync()
        {
            if (!this.ModelState.IsValid)
            {
                return this.Page();
            }

            var user = await this.signInManager.UserManager.FindByNameAsync(this.Input.Email);
            if (user == null)
            {
                this.ShowErrorMessage(NotificationMessages.TryAgainLaterError);
                return this.Page();
            }

            bool isEmailConfirmed = await this.signInManager.UserManager.IsEmailConfirmedAsync(user);
            if (isEmailConfirmed)
            {
                this.ShowErrorMessage(NotificationMessages.EmailAlreadyVerified);
                return this.RedirectToLoginPage();
            }

            var code = await this.userManager.GenerateEmailConfirmationTokenAsync(user);
            var callbackUrl = this.Url.Page(
                EmailMessages.EmailConfirmationPage,
                null,
                new { userId = user.Id, code },
                this.Request.Scheme);
            await this.emailSender.SendEmailAsync(GlobalConstants.BankSystemEmail, this.Input.Email,
                EmailMessages.ConfirmEmailSubject,
                string.Format(EmailMessages.EmailConfirmationMessage, HtmlEncoder.Default.Encode(callbackUrl)));

            this.ShowSuccessMessage(NotificationMessages.EmailVerificationLinkResentSuccessfully);
            return this.RedirectToHome();
        }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }
        }

        public class ReCaptchaModel : BaseReCaptchaModel
        {
        }
    }
}