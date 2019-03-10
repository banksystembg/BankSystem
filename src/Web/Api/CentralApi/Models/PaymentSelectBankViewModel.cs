namespace CentralApi.Models
{
    using System.Collections.Generic;

    public class PaymentSelectBankViewModel
    {
        public decimal Amount { get; set; }

        public string Description { get; set; }

        public IEnumerable<BankListingViewModel> Banks { get; set; }
    }
}