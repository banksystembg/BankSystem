namespace BankSystem.Web.Models.BankAccount
{
    using Areas.MoneyTransfers.Models;
    using Infrastructure.Collections;

    public class BankAccountDetailsViewModel
    {
        public string BankAccountUniqueId { get; set; }

        public PaginatedList<MoneyTransferListingDto> MoneyTransfers { get; set; }
    }
}
