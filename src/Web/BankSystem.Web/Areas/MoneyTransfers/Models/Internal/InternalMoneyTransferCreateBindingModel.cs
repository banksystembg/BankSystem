namespace BankSystem.Web.Areas.MoneyTransfers.Models.Internal
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Common;
    using Common.AutoMapping.Interfaces;
    using Microsoft.Extensions.DependencyInjection;
    using Services.BankAccount;
    using Services.Models.MoneyTransfer;
    using Web.Models.BankAccount;

    public class InternalMoneyTransferCreateBindingModel : IMapWith<MoneyTransferCreateServiceModel>, IValidatableObject
    {
        private const string DestinationAccountIncorrectError =
            "Destination account is incorrect or belongs to a different bank";

        [MaxLength(ModelConstants.MoneyTransfer.DescriptionMaxLength)]
        public string Description { get; set; }

        [Required]
        [Range(typeof(decimal), ModelConstants.MoneyTransfer.MinStartingPrice,
            ModelConstants.MoneyTransfer.MaxStartingPrice, ErrorMessage =
                "The amount cannot be lower than 0.01")]
        public decimal Amount { get; set; }

        [Required]
        [Display(Name = "Destination account")]
        [RegularExpression(@"^[A-Z]{4}\d{8}$", ErrorMessage = DestinationAccountIncorrectError)]
        public string DestinationBankAccountUniqueId { get; set; }

        public IEnumerable<OwnBankAccountListingViewModel> OwnAccounts { get; set; }

        [Required]
        [Display(Name = "Source account")]
        public string AccountId { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var uniqueIdHelper = validationContext.GetService<IBankAccountUniqueIdHelper>();
            if (!uniqueIdHelper.IsUniqueIdValid(this.DestinationBankAccountUniqueId))
            {
                yield return new ValidationResult(DestinationAccountIncorrectError,
                    new[] {nameof(this.DestinationBankAccountUniqueId)});
            }
        }
    }
}