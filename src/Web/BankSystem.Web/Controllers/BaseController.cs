namespace BankSystem.Web.Controllers
{
    using System.Linq;
    using System.Security.Claims;
    using Common;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Authorize]
    public abstract class BaseController : Controller
    {
        protected IActionResult RedirectToHome() => this.RedirectToAction("Index", "Home");

        protected string GetCurrentUserId()
        {
            if (!this.User.Identity.IsAuthenticated)
            {
                return null;
            }

            var claim = this.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            return claim?.Value;
        }

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