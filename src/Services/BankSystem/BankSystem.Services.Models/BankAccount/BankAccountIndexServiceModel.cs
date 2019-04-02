namespace BankSystem.Services.Models.BankAccount
{
    public class BankAccountIndexServiceModel : BankAccountBaseServiceModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public decimal Balance { get; set; }

        public string UniqueId { get; set; }
    }
}