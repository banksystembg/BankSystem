namespace BankSystem.Services.Models.MoneyTransfer
{
    using BankSystem.Models;
    using Common.AutoMapping.Interfaces;

    public class MoneyTransferBaseServiceModel : IMapWith<MoneyTransfer>
    {
        public string Id { get; set; }
    }
}
