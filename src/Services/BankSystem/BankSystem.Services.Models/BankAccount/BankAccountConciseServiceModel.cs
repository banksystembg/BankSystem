namespace BankSystem.Services.Models.BankAccount
{
    public class BankAccountConciseServiceModel : BankAccountBaseServiceModel
    {
        public string Id { get; set; }

        public string UniqueId { get; set; }

        public decimal Balance { get; set; }

        public string UserFullName { get; set; }
    }
}