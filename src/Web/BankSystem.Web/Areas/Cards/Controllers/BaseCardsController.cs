namespace BankSystem.Web.Areas.Cards.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Web.Controllers;

    [Area("Cards")]
    [Route("[area]/[action]/{id?}")]
    public abstract class BaseCardsController : BaseController
    {
    }
}