namespace BankSystem.Web.Areas.Administration.Models
{
    using Infrastructure.Collections;

    public class AllBankAccountsListViewModel
    {
        public PaginatedList<BankAccountListingViewModel> BankAccounts { get; set; }
    }
}