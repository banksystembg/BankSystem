namespace BankSystem.Services.Models.BankAccount
{
    public class BankAccountIndexServiceModel : BankAccountBaseServiceModel
    {
        public decimal Balance { get; set; }

        public string UniqueId { get; set; }
    }
}
