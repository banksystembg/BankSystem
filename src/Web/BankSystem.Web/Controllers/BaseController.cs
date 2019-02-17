namespace BankSystem.Web.Controllers
{
    using Common;
    using Microsoft.AspNetCore.Mvc;

    public abstract class BaseController : Controller
    {
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