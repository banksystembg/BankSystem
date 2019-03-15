namespace BankSystem.Web.Api
{
    using Areas.MoneyTransfers.Models.Global;
    using AutoMapper;
    using Common;
    using Common.Utils;
    using Common.Utils.CustomHandlers;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Newtonsoft.Json;
    using Services.Interfaces;
    using Services.Models.BankAccount;
    using Services.Models.MoneyTransfer;
    using System;
    using System.Net.Http;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading.Tasks;

    [Route("api/[controller]")]
    [ApiController]
    public class CardPaymentsController : ControllerBase
    {
        private readonly IBankConfigurationHelper bankConfigurationHelper;
        private readonly IBankAccountService bankAccountService;
        private readonly IMoneyTransferService moneyTransferService;

        public CardPaymentsController(
            IBankConfigurationHelper bankConfigurationHelper,
            IBankAccountService bankAccountService,
            IMoneyTransferService moneyTransferService)
        {
            this.bankConfigurationHelper = bankConfigurationHelper;
            this.bankAccountService = bankAccountService;
            this.moneyTransferService = moneyTransferService;
        }

        // POST: api/CardPayments
        [HttpPost("{data}", Name = "Post")]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Post([FromBody] string data)
        {
            // read and validate payment data
            data = Encoding.UTF8.GetString(Convert.FromBase64String(data));
            var deserializedJson = JsonConvert.DeserializeObject<ReceiveCardPaymentModel>(data);

            if (!this.ValidateSignatures(deserializedJson))
            {
                return this.PaymentFailed(NotificationMessages.PaymentStateInvalid);
            }

            var paymentInfo = JsonConvert.DeserializeObject<PaymentInfoModel>(deserializedJson.PaymentInfo);
            if (paymentInfo.Amount <= 0)
            {
                return this.PaymentFailed(NotificationMessages.PaymentStateInvalid);
            }

            var accountId = await this.bankAccountService.GetAccountIdAsync(paymentInfo.Number, paymentInfo.ParsedExpiryDate, paymentInfo.SecurityCode,
                paymentInfo.Name);
            // transfer money to destination account
            var error = await this.ExecuteTransfer(paymentInfo, accountId);

            if (error != null)
            {
                return error;
            }

            // return signed success response
            var responseJson = this.GenerateSuccessResponseJson(deserializedJson);
            var encodedResponse = Convert.ToBase64String(Encoding.UTF8.GetBytes(responseJson));

            return this.Ok(encodedResponse);
        }

        private string GenerateSuccessResponseJson(ReceiveCardPaymentModel deserializedJson)
        {
            // generate PaymentConfirmation
            var paymentConfirmation = new
            {
                deserializedJson.PaymentProofSignature
            };

            var paymentConfirmationJson = JsonConvert.SerializeObject(paymentConfirmation);

            // sign PaymentConfirmation
            string paymentConfirmationSignature;

            using (var bankRsa = RSA.Create())
            {
                RsaExtensions.FromXmlString(bankRsa, this.bankConfigurationHelper.Key);

                paymentConfirmationSignature = Convert.ToBase64String(
                    bankRsa.SignData(
                        Encoding.UTF8.GetBytes(paymentConfirmationJson),
                        HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1));
            }

            // generate response
            var response = new
            {
                deserializedJson.PaymentInfo,
                deserializedJson.PaymentProof,
                PaymentConfirmation = paymentConfirmationJson,
                PaymentConfirmationSignature = paymentConfirmationSignature
            };

            var responseJson = JsonConvert.SerializeObject(response);
            return responseJson;
        }

        private async Task<IActionResult> ExecuteTransfer(PaymentInfoModel paymentInfo, string sourceAccountId)
        {
            var account = await this.bankAccountService
                .GetBankAccountAsync<BankAccountIndexServiceModel>(sourceAccountId);

            var transferModel = new GlobalMoneyTransferCentralApiBindingModel
            {
                Amount = paymentInfo.Amount,
                Description = paymentInfo.Description,
                DestinationBankName = paymentInfo.DestinationBankName,
                DestinationBankCountry = paymentInfo.DestinationBankCountry,
                DestinationBankSwiftCode = paymentInfo.DestinationBankSwiftCode,
                DestinationBankAccountUniqueId = paymentInfo.DestinationBankAccountUniqueId,
                SenderName = account.UserFullName,
                SenderAccountUniqueId = account.UniqueId
            };

            // verify there is enough money in the account
            if (account.Balance < transferModel.Amount)
            {
                return this.PaymentFailed(NotificationMessages.InsufficientFunds);
            }

            // contact central api
            var response = await this.ContactCentralApiAsync(transferModel);
            if (!response.IsSuccessStatusCode)
            {
                return this.PaymentFailed(NotificationMessages.PaymentFailed);
            }

            // remove money from source account
            var serviceModel = Mapper.Map<MoneyTransferCreateServiceModel>(transferModel);
            serviceModel.Source = account.UniqueId;
            serviceModel.AccountId = sourceAccountId;
            serviceModel.Amount *= -1;

            var success = await this.moneyTransferService.CreateMoneyTransferAsync(serviceModel);
            return !success ? this.PaymentFailed(NotificationMessages.PaymentFailed) : null;
        }

        private IActionResult PaymentFailed(string message)
        {
            return this.BadRequest(message);
        }

        private bool ValidateSignatures(ReceiveCardPaymentModel data)
        {
            var paymentInfoJson = data.PaymentInfo;
            var centralApiPaymentInfoSignature = data.PaymentInfoSignature;
            var paymentProofJson = data.PaymentProof;
            var paymentProofSignature = data.PaymentProofSignature;

            // validate signatures of PaymentInfo and PaymentProof
            using (var centralApiRsa = RSA.Create())
            {
                RsaExtensions.FromXmlString(centralApiRsa, this.bankConfigurationHelper.CentralApiPublicKey);

                var isPaymentInfoSignatureValid = centralApiRsa.VerifyData(
                    Encoding.UTF8.GetBytes(paymentInfoJson),
                    Convert.FromBase64String(centralApiPaymentInfoSignature),
                    HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

                var isPaymentProofSignatureValid = centralApiRsa.VerifyData(
                    Encoding.UTF8.GetBytes(paymentProofJson),
                    Convert.FromBase64String(paymentProofSignature),
                    HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

                return isPaymentInfoSignatureValid && isPaymentProofSignatureValid;
            }
        }

        private async Task<HttpResponseMessage> ContactCentralApiAsync(GlobalMoneyTransferCentralApiBindingModel model)
        {
            var customHandler = new CustomDelegatingHandler(this.bankConfigurationHelper.CentralApiPublicKey,
                this.bankConfigurationHelper.Key,
                WebConstants.BankName, this.bankConfigurationHelper.UniqueIdentifier);
            var client = HttpClientFactory.Create(customHandler);

            return await client.PostAsJsonAsync($"{GlobalConstants.CentralApiBaseAddress}api/ReceiveTransactions",
                model);
        }
    }
}
