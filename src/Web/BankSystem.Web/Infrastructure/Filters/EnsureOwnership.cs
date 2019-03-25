namespace BankSystem.Web.Infrastructure.Filters
{
    using Areas.MoneyTransfers.Models;
    using BankSystem.Models;
    using Common.Utils;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Services.Implementations;
    using Services.Interfaces;
    using Services.Models.BankAccount;

    public class EnsureOwnership : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var httpContext = context.HttpContext;

            var userManager = httpContext.RequestServices.GetService(typeof(UserManager<BankUser>)) as UserManager<BankUser>;
            var bankAccountService = httpContext.RequestServices.GetService(typeof(IBankAccountService)) as BankAccountService;
            var userId = userManager?.GetUserId(context.HttpContext.User);

            var model = ActionArgumentsUtil.GetModel(context.ActionArguments) as IMoneyTransferCreateBindingModel;
            var dbAccount = bankAccountService?
                .GetByIdAsync<BankAccountVerifyOwnershipServiceModel>(model?.AccountId)
                .GetAwaiter()
                .GetResult();
            if (dbAccount == null || dbAccount.UserId != userId)
            {
                context.Result = new ForbidResult();
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}
