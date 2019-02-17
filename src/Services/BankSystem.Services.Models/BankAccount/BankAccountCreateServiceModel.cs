namespace BankSystem.Services.Models.BankAccount
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class BankAccountCreateServiceModel : BankAccountBaseServiceModel
    {
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    }
}