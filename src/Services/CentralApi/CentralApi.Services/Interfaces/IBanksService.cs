namespace CentralApi.Services.Interfaces
{
    using System.Threading.Tasks;
    using Models.Banks;

    public interface IBanksService
    {
        Task<T> GetBankAsync<T>(string bankName, string swiftCode, string bankCountry)
            where T : BankBaseServiceModel;
    }
}
