namespace BankSystem.Services.Implementations
{
    using AutoMapper;
    using Common.AutoMapping.Interfaces;
    using Common.Utils;
    using Common.Utils.CustomHandlers;
    using Interfaces;
    using Models.BankAccount;
    using Models.GlobalTransfer;
    using Models.MoneyTransfer;
    using System.Net.Http;
    using System.Threading.Tasks;

    public class GlobalTransferHelper : IGlobalTransferHelper
    {
        private const string CentralApiTransferSubmitUrlFormat = "{0}api/ReceiveTransactions";

        private readonly IBankAccountService bankAccountService;
        private readonly IBankConfigurationHelper bankConfigurationHelper;
        private readonly IMoneyTransferService moneyTransferService;

        public GlobalTransferHelper(
            IBankAccountService bankAccountService,
            IMoneyTransferService moneyTransferService,
            IBankConfigurationHelper bankConfigurationHelper)
        {
            this.bankAccountService = bankAccountService;
            this.moneyTransferService = moneyTransferService;
            this.bankConfigurationHelper = bankConfigurationHelper;
        }

        public async Task<GlobalTransferResult> TransferMoneyAsync(GlobalTransferServiceModel model)
        {
            if (!ValidationUtil.IsObjectValid(model))
            {
                return GlobalTransferResult.GeneralFailure;
            }

            var account = await this.bankAccountService
                .GetByIdAsync<BankAccountConciseServiceModel>(model.SourceAccountId);

            // check if account exists and recipient name is accurate
            if (account == null)
            {
                return GlobalTransferResult.GeneralFailure;
            }

            // verify there is enough money in the account
            if (account.Balance < model.Amount)
            {
                return GlobalTransferResult.InsufficientFunds;
            }

            // contact the CentralApi to execute the transfer
            var submitDto = Mapper.Map<CentralApiSubmitTransferDto>(model);
            submitDto.SenderName = account.UserFullName;
            submitDto.SenderAccountUniqueId = account.UniqueId;

            bool remoteSuccess = await this.ContactCentralApiAsync(submitDto);

            if (!remoteSuccess)
            {
                return GlobalTransferResult.GeneralFailure;
            }

            // remove money from source account
            var serviceModel = new MoneyTransferCreateServiceModel
            {
                Amount = -model.Amount,
                Source = account.UniqueId,
                Description = model.Description,
                AccountId = account.Id,
                DestinationBankAccountUniqueId = model.DestinationBankAccountUniqueId,
                SenderName = account.UserFullName,
                RecipientName = model.RecipientName
            };

            bool success = await this.moneyTransferService.CreateMoneyTransferAsync(serviceModel);
            return !success ? GlobalTransferResult.GeneralFailure : GlobalTransferResult.Succeeded;
        }

        private async Task<bool> ContactCentralApiAsync(CentralApiSubmitTransferDto model)
        {
            var customHandler = new CustomDelegatingHandler(
                this.bankConfigurationHelper.CentralApiPublicKey,
                this.bankConfigurationHelper.Key,
                this.bankConfigurationHelper.BankName,
                this.bankConfigurationHelper.UniqueIdentifier,
                this.bankConfigurationHelper.BankCountry);

            var client = HttpClientFactory.Create(customHandler);

            var response = await client.PostAsJsonAsync(
                string.Format(CentralApiTransferSubmitUrlFormat, this.bankConfigurationHelper.CentralApiAddress),
                model);

            return response.IsSuccessStatusCode;
        }

        private class CentralApiSubmitTransferDto : IMapWith<GlobalTransferServiceModel>
        {
            public string Description { get; set; }

            public decimal Amount { get; set; }

            public string SenderName { get; set; }

            public string RecipientName { get; set; }

            public string SenderAccountUniqueId { get; set; }

            public string DestinationBankSwiftCode { get; set; }

            public string DestinationBankName { get; set; }

            public string DestinationBankCountry { get; set; }

            public string DestinationBankAccountUniqueId { get; set; }
        }
    }
}