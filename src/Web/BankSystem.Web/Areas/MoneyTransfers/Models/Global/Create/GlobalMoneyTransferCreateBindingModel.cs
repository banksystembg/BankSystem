namespace BankSystem.Web.Areas.MoneyTransfers.Models.Global.Create
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Common;
    using Common.AutoMapping.Interfaces;
    using Infrastructure.Helpers.GlobalTransferHelpers.Models;
    using Web.Models.BankAccount;

    public class GlobalMoneyTransferCreateBindingModel : IMapWith<GlobalTransferDto>
    {
        [Required]
        public GlobalMoneyTransferCreateDestinationBankDto DestinationBank { get; set; }

        [MaxLength(ModelConstants.MoneyTransfer.DescriptionMaxLength)]
        [Display(Name = "Details of Payment")]
        public string Description { get; set; }

        [Required]
        [Range(typeof(decimal), ModelConstants.MoneyTransfer.MinStartingPrice,
            ModelConstants.MoneyTransfer.MaxStartingPrice,
            ErrorMessage = "The Amount field cannot be lower than 0.01")]
        public decimal Amount { get; set; }

        [Display(Name = "Name")]
        [MaxLength(ModelConstants.User.FullNameMaxLength)]
        public string SenderName { get; set; }

        public IEnumerable<OwnBankAccountListingViewModel> OwnAccounts { get; set; }

        [Required]
        [Display(Name = "From Account")]
        public string AccountId { get; set; }
    }
}