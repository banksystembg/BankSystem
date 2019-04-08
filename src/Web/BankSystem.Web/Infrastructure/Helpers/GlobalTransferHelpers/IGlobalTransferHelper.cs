namespace BankSystem.Web.Infrastructure.Helpers.GlobalTransferHelpers
{
    using System.Threading.Tasks;
    using Models;

    public interface IGlobalTransferHelper
    {
        Task<GlobalTransferResult> TransferMoneyAsync(GlobalTransferDto model);
    }
}