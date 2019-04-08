namespace BankSystem.Web.Controllers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;
    using AutoMapper;
    using Common;
    using Common.Configuration;
    using Infrastructure.Helpers;
    using Infrastructure.Helpers.GlobalTransferHelpers;
    using Infrastructure.Helpers.GlobalTransferHelpers.Models;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using Models;
    using Models.BankAccount;
    using Services.Interfaces;
    using Services.Models.BankAccount;

    [Authorize]
    public class PaymentsController : BaseController
    {
        private const int CookieValidityInMinutes = 5;
        private const string PaymentDataCookie = "PaymentData";
        private readonly IBankAccountService bankAccountService;

        private readonly BankConfiguration bankConfiguration;
        private readonly IGlobalTransferHelper globalTransferHelper;
        private readonly IUserService userService;

        public PaymentsController(
            IOptions<BankConfiguration> bankConfigurationOptions,
            IBankAccountService bankAccountService,
            IUserService userService,
            IGlobalTransferHelper globalTransferHelper)
        {
            this.bankConfiguration = bankConfigurationOptions.Value;
            this.bankAccountService = bankAccountService;
            this.userService = userService;
            this.globalTransferHelper = globalTransferHelper;
        }

        [HttpPost]
        [AllowAnonymous]
        [IgnoreAntiforgeryToken]
        [Route("/pay")]
        public IActionResult SetCookie(string data)
        {
            string decodedData;

            try
            {
                decodedData = DirectPaymentsHelper.DecodePaymentRequest(data);
            }
            catch
            {
                return this.BadRequest();
            }

            // set payment data cookie
            this.Response.Cookies.Append(PaymentDataCookie, decodedData,
                new CookieOptions
                {
                    SameSite = SameSiteMode.Lax,
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
                dynamic paymentRequest =
                    DirectPaymentsHelper.ParsePaymentRequest(data, this.bankConfiguration.CentralApiPublicKey);
                if (paymentRequest == null)
                {
                    return this.BadRequest();
                }

                dynamic paymentInfo = DirectPaymentsHelper.GetPaymentInfo(paymentRequest);

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
                    DataHash = DirectPaymentsHelper.Sha256Hash(data)
                };

                return this.View(model);
            }
            catch
            {
                return this.BadRequest();
            }
        }

        [HttpPost]
        public async Task<IActionResult> PayAsync(PaymentConfirmBindingModel model)
        {
            bool cookieExists = this.Request.Cookies.TryGetValue(PaymentDataCookie, out var data);

            if (!this.ModelState.IsValid ||
                !cookieExists ||
                model.DataHash != DirectPaymentsHelper.Sha256Hash(data))
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
                dynamic paymentRequest =
                    DirectPaymentsHelper.ParsePaymentRequest(data, this.bankConfiguration.CentralApiPublicKey);

                if (paymentRequest == null)
                {
                    return this.PaymentFailed(NotificationMessages.PaymentStateInvalid);
                }

                dynamic paymentInfo = DirectPaymentsHelper.GetPaymentInfo(paymentRequest);

                string returnUrl = paymentRequest.ReturnUrl;

                // transfer money to destination account
                var serviceModel = new GlobalTransferDto
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
                    return this.PaymentFailed(result == GlobalTransferResult.InsufficientFunds
                        ? NotificationMessages.InsufficientFunds
                        : NotificationMessages.TryAgainLaterError);
                }

                // delete cookie to prevent accidental duplicate payments
                this.Response.Cookies.Delete(PaymentDataCookie);

                // return signed success response
                var response = DirectPaymentsHelper.GenerateSuccessResponse(paymentRequest,
                    this.bankConfiguration.Key);

                return this.Ok(new
                {
                    success = true,
                    returnUrl = HttpUtility.HtmlEncode(returnUrl),
                    data = response
                });
            }
            catch
            {
                return this.PaymentFailed(NotificationMessages.PaymentStateInvalid);
            }
        }

        private IActionResult PaymentFailed(string message)
        {
            return this.Ok(new
            {
                success = false,
                errorMessage = message
            });
        }

        private async Task<OwnBankAccountListingViewModel[]> GetAllAccountsAsync(string userId)
        {
            return (await this.bankAccountService
                    .GetAllAccountsByUserIdAsync<BankAccountIndexServiceModel>(userId))
                .Select(Mapper.Map<OwnBankAccountListingViewModel>)
                .ToArray();
        }
    }
}