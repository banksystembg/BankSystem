namespace BankSystem.Web.Areas.MoneyTransfers.Models
{
    using Common.AutoMapping.Interfaces;
    using Services.Models.MoneyTransfer;

    public interface IMoneyTransferCreateBindingModel : IMapWith<MoneyTransferCreateServiceModel>
    {
        string AccountId { get; set; }
    }
}