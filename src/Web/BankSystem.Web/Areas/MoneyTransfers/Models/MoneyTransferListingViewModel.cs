namespace BankSystem.Web.Areas.MoneyTransfers.Models
{
    using Infrastructure.Collections;

    public class MoneyTransferListingViewModel
    {
        public PaginatedList<MoneyTransferListingDto> MoneyTransfers { get; set; }
    }
}