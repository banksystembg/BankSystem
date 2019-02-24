namespace BankSystem.Web.Infrastructure.Filters
{
    using System.Linq;
    using BankSystem.Models;
    using Data;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.EntityFrameworkCore;
    using Models.MoneyTransfers;

    public class EnsureOwnership : ActionFilterAttribute
    {
        private const string Id = "model";

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var httpContext = context.HttpContext;

            var database = httpContext.RequestServices.GetService(typeof(BankSystemDbContext)) as BankSystemDbContext;
            var userManager = httpContext.RequestServices.GetService(typeof(UserManager<BankUser>)) as UserManager<BankUser>;
            var userId = userManager?.GetUserId(context.HttpContext.User);
            var model = context.ActionArguments[Id] as IMoneyTransferCreateBindingModel;

            var dbAccount = database?.Accounts.AsNoTracking().FirstOrDefault(v => v.Id == model.AccountId);
            if (dbAccount == null || dbAccount.UserId != userId)
            {
                context.Result = new ForbidResult();
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}
