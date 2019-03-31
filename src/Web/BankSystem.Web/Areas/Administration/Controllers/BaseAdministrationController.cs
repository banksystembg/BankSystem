namespace BankSystem.Web.Areas.Administration.Controllers
{
    using Common;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Web.Controllers;

    [Area("Administration")]
    [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
    public abstract class BaseAdministrationController : BaseController
    {
    }
}