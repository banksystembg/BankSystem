namespace BankSystem.Web.Pages
{
    using Common;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;

    public abstract class BasePageModel : PageModel
    {
        protected const string EmailSubject = "Confirm your email";
        protected const string EmailMessage = "Please confirm your email by <a href=\"{0}\">clicking here</a>.";
        protected const string EmailConfirmationPage = "/Account/ConfirmEmail";

        public IActionResult RedirectToHome() => this.RedirectToAction("Index", "Home");

        protected void ShowErrorMessage(string message)
        {
            this.TempData[GlobalConstants.TempDataErrorMessageKey] = message;
        }

        protected void ShowSuccessMessage(string message)
        {
            this.TempData[GlobalConstants.TempDataSuccessMessageKey] = message;
        }
    }
}