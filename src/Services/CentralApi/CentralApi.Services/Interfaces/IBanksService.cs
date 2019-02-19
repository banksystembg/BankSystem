namespace CentralApi.Services.Interfaces
{
    using System.Threading.Tasks;

    public interface IBanksService
    {
        Task<bool> CheckWhetherBankExistsAsync(string bankName, string swiftCode, string bankCountry);
    }
}
