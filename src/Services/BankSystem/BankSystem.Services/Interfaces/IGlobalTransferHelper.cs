namespace BankSystem.Services.Interfaces
{
    using System.Threading.Tasks;
    using Models.GlobalTransfer;

    public interface IGlobalTransferHelper
    {
        Task<GlobalTransferResult> TransferMoneyAsync(GlobalTransferServiceModel model);
    }
}