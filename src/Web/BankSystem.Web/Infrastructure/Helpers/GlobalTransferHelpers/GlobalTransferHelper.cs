namespace BankSystem.Web.Infrastructure.Helpers.GlobalTransferHelpers
{
    using System;
    using System.Net.Http;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading.Tasks;
    using AutoMapper;
    using Common.Configuration;
    using Common.Utils;
    using Microsoft.Extensions.Options;
    using Models;
    using Newtonsoft.Json;
    using Services.BankAccount;
    using Services.Models.BankAccount;
    using Services.Models.MoneyTransfer;
    using Services.MoneyTransfer;

    public class GlobalTransferHelper : IGlobalTransferHelper
    {
        private const string CentralApiTransferSubmitUrlFormat = "{0}api/ReceiveTransactions";

        private readonly IBankAccountService bankAccountService;
        private readonly BankConfiguration bankConfiguration;
        private readonly IMoneyTransferService moneyTransferService;
        private readonly IMapper mapper;

        public GlobalTransferHelper(
            IBankAccountService bankAccountService,
            IMoneyTransferService moneyTransferService,
            IOptions<BankConfiguration> bankConfigurationOptions,
            IMapper mapper)
        {
            this.bankAccountService = bankAccountService;
            this.moneyTransferService = moneyTransferService;
            this.mapper = mapper;
            this.bankConfiguration = bankConfigurationOptions.Value;
        }

        public async Task<GlobalTransferResult> TransferMoneyAsync(GlobalTransferDto model)
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
            var submitDto = this.mapper.Map<CentralApiSubmitTransferDto>(model);
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
                RecipientName = model.RecipientName,
                ReferenceNumber = submitDto.ReferenceNumber
            };

            bool success = await this.moneyTransferService.CreateMoneyTransferAsync(serviceModel);
            return !success ? GlobalTransferResult.GeneralFailure : GlobalTransferResult.Succeeded;
        }

        private async Task<bool> ContactCentralApiAsync(CentralApiSubmitTransferDto model)
        {
            var encryptedData = this.SignAndEncryptData(model);

            var client = new HttpClient();
            var response = await client.PostAsJsonAsync(
                string.Format(CentralApiTransferSubmitUrlFormat, this.bankConfiguration.CentralApiAddress),
                encryptedData);

            return response != null && response.IsSuccessStatusCode;
        }

        private string SignAndEncryptData(CentralApiSubmitTransferDto model)
        {
            using var rsa = RSA.Create();
            RsaExtensions.FromXmlString(rsa, this.bankConfiguration.Key);
            var aesParams = CryptographyExtensions.GenerateKey();
            var key = Convert.FromBase64String(aesParams[0]);
            var iv = Convert.FromBase64String(aesParams[1]);

            var serializedModel = JsonConvert.SerializeObject(model);
            var dataObject = new
            {
                Model = serializedModel,
                Timestamp = DateTime.UtcNow
            };

            var data = JsonConvert.SerializeObject(dataObject);

            var signature = Convert.ToBase64String(rsa
                .SignData(Encoding.UTF8.GetBytes(data), HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1));

            string encryptedKey;
            string encryptedIv;
            using (var encryptionRsa = RSA.Create())
            {
                RsaExtensions.FromXmlString(encryptionRsa, this.bankConfiguration.CentralApiPublicKey);
                encryptedKey = Convert.ToBase64String(encryptionRsa.Encrypt(key, RSAEncryptionPadding.Pkcs1));
                encryptedIv = Convert.ToBase64String(encryptionRsa.Encrypt(iv, RSAEncryptionPadding.Pkcs1));
            }

            var encryptedData = Convert.ToBase64String(CryptographyExtensions.Encrypt(data, key, iv));

            var json = new
            {
                BankName = this.bankConfiguration.BankName,
                BankSwiftCode = this.bankConfiguration.UniqueIdentifier,
                BankCountry = this.bankConfiguration.Country,
                EncryptedKey = encryptedKey,
                EncryptedIv = encryptedIv,
                Data = encryptedData,
                Signature = signature
            };

            var jsonRequest = JsonConvert.SerializeObject(json);
            var encryptedRequest = Convert.ToBase64String(Encoding.UTF8.GetBytes(jsonRequest));

            return encryptedRequest;
        }
    }
}