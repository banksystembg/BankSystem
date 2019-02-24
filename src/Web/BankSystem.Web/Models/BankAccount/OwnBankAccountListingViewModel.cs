namespace BankSystem.Web.Models.BankAccount
{
    using Common.AutoMapping.Interfaces;
    using Services.Models.BankAccount;

    public class OwnBankAccountListingViewModel : IMapWith<BankAccountIndexServiceModel>
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public decimal Balance { get; set; }

        public string UniqueId { get; set; }
    }
}