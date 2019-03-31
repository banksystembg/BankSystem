namespace BankSystem.Web.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using BankAccount;

    public class PaymentConfirmBindingModel
    {
        public decimal Amount { get; set; }

        public string Description { get; set; }

        public string DestinationBankName { get; set; }

        public string DestinationBankCountry { get; set; }

        public string DestinationBankAccountUniqueId { get; set; }

        public string RecipientName { get; set; }

        public IEnumerable<OwnBankAccountListingViewModel> OwnAccounts { get; set; }

        [Required]
        public string DataHash { get; set; }

        [Required]
        [Display(Name = "Source account")]
        public string AccountId { get; set; }
    }
}