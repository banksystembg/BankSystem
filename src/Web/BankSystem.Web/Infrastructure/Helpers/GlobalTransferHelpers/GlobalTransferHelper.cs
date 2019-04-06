namespace BankSystem.Web.Infrastructure.Helpers.GlobalTransferHelpers
{
    using System;
    using System.Net.Http;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading.Tasks;
    using AutoMapper;
    using Common.Utils;
    using Models;
    using Newtonsoft.Json;
    using Services.Interfaces;
    using Services.Models.BankAccount;
    using Services.Models.MoneyTransfer;

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
            var encryptedData = this.SignAndEncryptData(model);

            var client = new HttpClient();
            var response = await client.PostAsJsonAsync(
                string.Format(CentralApiTransferSubmitUrlFormat, this.bankConfigurationHelper.CentralApiAddress),
                encryptedData);

            return response.IsSuccessStatusCode;
        }

        private string SignAndEncryptData(CentralApiSubmitTransferDto model)
        {
            using (var rsa = RSA.Create())
            {
                RsaExtensions.FromXmlString(rsa, this.bankConfigurationHelper.Key);
                var aesParams = CryptographyExtensions.GenerateKey();
                var key = Convert.FromBase64String(aesParams[0]);
                var iv = Convert.FromBase64String(aesParams[1]);

                var signedData = rsa
                    .SignData(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(model)), HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

                string encryptedKey;
                string encryptedIv;
                using (var encryptionRsa = RSA.Create())
                {
                    RsaExtensions.FromXmlString(encryptionRsa, this.bankConfigurationHelper.CentralApiPublicKey);
                    encryptedKey = Convert.ToBase64String(encryptionRsa.Encrypt(key, RSAEncryptionPadding.Pkcs1));
                    encryptedIv = Convert.ToBase64String(encryptionRsa.Encrypt(iv, RSAEncryptionPadding.Pkcs1));
                }

                var json = new
                {
                    this.bankConfigurationHelper.BankName,
                    BankSwiftCode = this.bankConfigurationHelper.UniqueIdentifier,
                    this.bankConfigurationHelper.BankCountry,
                    EncryptedKey = encryptedKey,
                    EncryptedIv = encryptedIv,
                    Data = Convert.ToBase64String(CryptographyExtensions.Encrypt(JsonConvert.SerializeObject(model), key, iv)),
                    SignedData = Convert.ToBase64String(signedData)
                };

                var jsonRequest = JsonConvert.SerializeObject(json);
                var encryptedData = Convert.ToBase64String(Encoding.UTF8.GetBytes(jsonRequest));

                return encryptedData;
            }
        }
    }
}