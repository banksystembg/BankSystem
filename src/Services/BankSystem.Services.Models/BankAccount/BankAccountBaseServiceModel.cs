namespace BankSystem.Services.Models.BankAccount
{
    using BankSystem.Models;
    using Common.AutoMapping.Interfaces;

    public class BankAccountBaseServiceModel : IMapWith<BankAccount>
    {
        public string Id { get; set; }
    }
}
