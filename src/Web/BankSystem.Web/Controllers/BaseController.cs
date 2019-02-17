namespace BankSystem.Web.Controllers
{
    using Common;
    using Microsoft.AspNetCore.Mvc;

    public abstract class BaseController : Controller
    {
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