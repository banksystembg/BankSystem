namespace CentralApi.Models
{
    using System.ComponentModel.DataAnnotations;
    using BankSystem.Common;
    using BankSystem.Common.AutoMapping.Interfaces;

    public class SendTransactionModel : IMapWith<ReceiveTransactionModel>
    {
        [MaxLength(ModelConstants.MoneyTransfer.DescriptionMaxLength)]
        public string Description { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        [MaxLength(ModelConstants.BankAccount.UniqueIdMaxLength)]
        public string DestinationBankAccountUniqueId { get; set; }

        [Required]
        [MaxLength(ModelConstants.User.FullNameMaxLength)]
        public string SenderName { get; set; }

        [Required]
        [MaxLength(ModelConstants.User.FullNameMaxLength)]
        public string RecipientName { get; set; }

        [Required]
        [MaxLength(ModelConstants.BankAccount.UniqueIdMaxLength)]
        public string SenderAccountUniqueId { get; set; }

        [Required]
        public string ReferenceNumber { get; set; }
    }
}