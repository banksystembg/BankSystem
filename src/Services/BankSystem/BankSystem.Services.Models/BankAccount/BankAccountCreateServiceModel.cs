namespace BankSystem.Services.Models.BankAccount
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Common;

    public class BankAccountCreateServiceModel : BankAccountBaseServiceModel
    {
        [MaxLength(ModelConstants.BankAccount.NameMaxLength)]
        public string Name { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    }
}