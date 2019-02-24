namespace BankSystem.Web.Models.InternalMoneyTransfer
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using BankAccount;
    using Microsoft.Extensions.DependencyInjection;
    using MoneyTransfers;
    using Services.Interfaces;

    public class InternalMoneyTransferCreateBindingModel : IMoneyTransferCreateBindingModel, IValidatableObject
    {
        private const string DestinationAccountIncorrectError =
            "Destination account is incorrect or belongs to a different bank";

        [MaxLength(150)]
        public string Description { get; set; }

        [Required]
        [Range(typeof(decimal), "0.01", "79228162514264337593543950335", ErrorMessage =
            "The amount cannot be lower than 0.01")]
        public decimal Amount { get; set; }

        [Required]
        [Display(Name = "Source account")]
        public string AccountId { get; set; }

        [Required]
        [Display(Name = "Destination account")]
        [RegularExpression(@"^[A-Z]{4}\d{8}$", ErrorMessage = DestinationAccountIncorrectError)]
        public string DestinationBankAccountUniqueId { get; set; }

        public IEnumerable<OwnBankAccountListingViewModel> OwnAccounts { get; set; }

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