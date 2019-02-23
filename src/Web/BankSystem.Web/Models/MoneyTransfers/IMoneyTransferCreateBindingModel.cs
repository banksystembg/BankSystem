namespace BankSystem.Web.Models.MoneyTransfers
{
    using Common.AutoMapping.Interfaces;
    using Services.Models.MoneyTransfer;

    public interface IMoneyTransferCreateBindingModel : IMapWith<MoneyTransferCreateServiceModel>
    {
        string AccountId { get; set; }
    }
}