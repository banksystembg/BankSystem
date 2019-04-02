namespace BankSystem.Services.Models.BankAccount
{
    using System;

    public class BankAccountDetailsServiceModel : BankAccountBaseServiceModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public decimal Balance { get; set; }

        public string UniqueId { get; set; }

        public DateTime CreatedOn { get; set; }

        public string UserId { get; set; }

        public string UserUserName { get; set; }

        public string UserFullName { get; set; }
    }
}