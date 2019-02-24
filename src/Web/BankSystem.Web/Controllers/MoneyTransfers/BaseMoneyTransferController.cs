namespace BankSystem.Web.Controllers.MoneyTransfers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Services.Interfaces;
    using Services.Models.BankAccount;

    public class BaseMoneyTransferController : BaseController
    {
        private readonly IBankAccountService bankAccountService;

        public BaseMoneyTransferController(IBankAccountService bankAccountService)
        {
            this.bankAccountService = bankAccountService;
        }

        protected async Task<IEnumerable<SelectListItem>> GetAllUserAccountsAsync(string userId)
        {
            var userAccounts = await this.bankAccountService
                .GetAllUserAccountsAsync<BankAccountIndexServiceModel>(userId);

            return userAccounts
                .Select(a => new SelectListItem { Text = $"{a.Name} - ({a.Balance} EUR)", Value = a.Id })
                .ToArray();
        }
    }
}
