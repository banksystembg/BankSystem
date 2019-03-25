namespace BankSystem.Web.Controllers
{
    using AutoMapper;
    using Common;
    using Common.Utils;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Models.BankAccount;
    using Newtonsoft.Json;
    using Services.Interfaces;
    using Services.Models.BankAccount;
    using Services.Models.GlobalTransfer;
    using System;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading.Tasks;

    [Authorize]
    public class PaymentsController : BaseController
    {
        private const int CookieValidityInMinutes = 5;
        private const string PaymentDataCookie = "PaymentData";
        private readonly IBankAccountService bankAccountService;

        private readonly IBankConfigurationHelper bankConfigurationHelper;
        private readonly IGlobalTransferHelper globalTransferHelper;
        private readonly IUserService userService;

        public PaymentsController(
            IBankConfigurationHelper bankConfigurationHelper,
            IBankAccountService bankAccountService,
            IUserService userService,
            IGlobalTransferHelper globalTransferHelper)
        {
            this.bankConfigurationHelper = bankConfigurationHelper;
            this.bankAccountService = bankAccountService;
            this.userService = userService;
            this.globalTransferHelper = globalTransferHelper;
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("/pay/{data}")]
        public IActionResult SetCookie(string data)
        {
            string decodedData;

            try
            {
                decodedData = Base64UrlUtil.Decode(data);
            }
            catch
            {
                return this.BadRequest();
            }

            // set payment data cookie
            this.Response.Cookies.Append(PaymentDataCookie, decodedData,
                new CookieOptions
                {
                    SameSite = SameSiteMode.Strict,
                    HttpOnly = false,
                    IsEssential = true,
                    MaxAge = TimeSpan.FromMinutes(CookieValidityInMinutes)
                });

            return this.RedirectToAction("Process");
        }

        [HttpGet]
        [Route("/pay")]
        public async Task<IActionResult> Process()
        {
            bool cookieExists = this.Request.Cookies.TryGetValue(PaymentDataCookie, out var data);

            if (!cookieExists)
            {
                return this.RedirectToHome();
            }

            try
            {
                dynamic deserializedJson = JsonConvert.DeserializeObject(data);
                string paymentInfoJson = deserializedJson.PaymentInfo;

                if (!ValidateSignatures(deserializedJson))
                {
                    return this.BadRequest();
                }

                dynamic paymentInfo = JsonConvert.DeserializeObject(paymentInfoJson);
                if (paymentInfo.Amount <= 0)
                {
                    return this.BadRequest();
                }

                var userId = await this.userService.GetUserIdByUsernameAsync(this.User.Identity.Name);

                var model = new PaymentConfirmBindingModel
                {
                    Amount = paymentInfo.Amount,
                    Description = paymentInfo.Description,
                    DestinationBankName = paymentInfo.DestinationBankName,
                    DestinationBankCountry = paymentInfo.DestinationBankCountry,
                    DestinationBankAccountUniqueId = paymentInfo.DestinationBankAccountUniqueId,
                    RecipientName = paymentInfo.RecipientName,
                    OwnAccounts = await this.GetAllAccountsAsync(userId),
                    DataHash = Sha256Hash(data)
                };

                return this.View(model);
            }
            catch
            {
                return this.BadRequest();
            }
        }

        [HttpPost]
        [Route("/pay")]
        public async Task<IActionResult> PayAsync(PaymentConfirmBindingModel model)
        {
            bool cookieExists = this.Request.Cookies.TryGetValue(PaymentDataCookie, out var data);

            if (!this.ModelState.IsValid ||
                !cookieExists ||
                model.DataHash != Sha256Hash(data))
            {
                return this.PaymentFailed(NotificationMessages.PaymentStateInvalid);
            }

            var account =
                await this.bankAccountService.GetByIdAsync<BankAccountDetailsServiceModel>(model.AccountId);
            if (account == null || account.UserUserName != this.User.Identity.Name)
            {
                return this.Forbid();
            }

            try
            {
                // read and validate payment data
                dynamic deserializedJson = JsonConvert.DeserializeObject(data);

                string paymentInfoJson = deserializedJson.PaymentInfo;
                string returnUrl = deserializedJson.ReturnUrl;

                if (!ValidateSignatures(deserializedJson))
                {
                    return this.PaymentFailed(NotificationMessages.PaymentStateInvalid);
                }

                dynamic paymentInfo = JsonConvert.DeserializeObject(paymentInfoJson);
                if (paymentInfo.Amount <= 0)
                {
                    return this.PaymentFailed(NotificationMessages.PaymentStateInvalid);
                }

                // transfer money to destination account
                var serviceModel = new GlobalTransferServiceModel
                {
                    Amount = paymentInfo.Amount,
                    Description = paymentInfo.Description,
                    DestinationBankName = paymentInfo.DestinationBankName,
                    DestinationBankCountry = paymentInfo.DestinationBankCountry,
                    DestinationBankSwiftCode = paymentInfo.DestinationBankSwiftCode,
                    DestinationBankAccountUniqueId = paymentInfo.DestinationBankAccountUniqueId,
                    RecipientName = paymentInfo.RecipientName,
                    SourceAccountId = model.AccountId
                };

                var result = await this.globalTransferHelper.TransferMoneyAsync(serviceModel);

                if (result != GlobalTransferResult.Succeeded)
                {
                    return this.PaymentFailed(result == GlobalTransferResult.InsufficientFunds ? NotificationMessages.InsufficientFunds
                        : NotificationMessages.TryAgainLaterError);
                }

                // delete cookie to prevent accidental duplicate payments
                this.Response.Cookies.Delete(PaymentDataCookie);

                // return signed success response
                var responseJson = this.GenerateSuccessResponseJson(deserializedJson);
                var encodedResponse = Base64UrlUtil.Encode(responseJson);

                return this.Ok(new
                {
                    success = true,
                    returnUrl = string.Format(returnUrl, encodedResponse)
                });
            }
            catch
            {
                return this.PaymentFailed(NotificationMessages.PaymentStateInvalid);
            }
        }

        private string GenerateSuccessResponseJson(dynamic deserializedJson)
        {
            // generate PaymentConfirmation
            var paymentConfirmation = new
            {
                Success = true,
                deserializedJson.PaymentProofSignature
            };

            string paymentConfirmationJson = JsonConvert.SerializeObject(paymentConfirmation);

            // sign the PaymentConfirmation
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

        private IActionResult PaymentFailed(string message)
        {
            return this.Ok(new
            {
                success = false,
                errorMessage = message
            });
        }

        private bool ValidateSignatures(dynamic data)
        {
            string paymentInfoJson = data.PaymentInfo;
            string centralApiPaymentInfoSignature = data.PaymentInfoSignature;
            string paymentProofJson = data.PaymentProof;
            string paymentProofSignature = data.PaymentProofSignature;

            // validate signatures of PaymentInfo and PaymentProof
            using (var centralApiRsa = RSA.Create())
            {
                RsaExtensions.FromXmlString(centralApiRsa, this.bankConfigurationHelper.CentralApiPublicKey);

                bool isPaymentInfoSignatureValid = centralApiRsa.VerifyData(
                    Encoding.UTF8.GetBytes(paymentInfoJson),
                    Convert.FromBase64String(centralApiPaymentInfoSignature),
                    HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

                bool isPaymentProofSignatureValid = centralApiRsa.VerifyData(
                    Encoding.UTF8.GetBytes(paymentProofJson),
                    Convert.FromBase64String(paymentProofSignature),
                    HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

                return isPaymentInfoSignatureValid && isPaymentProofSignatureValid;
            }
        }

        private async Task<OwnBankAccountListingViewModel[]> GetAllAccountsAsync(string userId)
        {
            return (await this.bankAccountService
                    .GetAllAccountsByUserIdAsync<BankAccountIndexServiceModel>(userId))
                .Select(Mapper.Map<OwnBankAccountListingViewModel>)
                .ToArray();
        }

        private static string Sha256Hash(string data)
        {
            using (var sha256Hash = SHA256.Create())
            {
                var bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(data));

                var builder = new StringBuilder();
                foreach (var bt in bytes)
                {
                    builder.Append(bt.ToString("x2"));
                }

                return builder.ToString();
            }
        }
    }
}