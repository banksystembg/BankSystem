namespace CentralApi.Controllers
{
    using System;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading.Tasks;
    using AutoMapper;
    using BankSystem.Common.Utils;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using Models;
    using Newtonsoft.Json;
    using Services.Interfaces;
    using Services.Models.Banks;

    public class PaymentsController : Controller
    {
        private const int CookieValidityInMinutes = 5;
        private const string PaymentDataCookie = "PaymentData";
        private const string PaymentDataFormKey = "data";

        private readonly IBanksService banksService;
        private readonly CentralApiConfiguration configuration;

        public PaymentsController(IBanksService banksService, IOptions<CentralApiConfiguration> configuration)
        {
            this.banksService = banksService;
            this.configuration = configuration.Value;
        }


        [HttpPost]
        [Route("/pay")]
        public IActionResult SetCookie(string data)
        {
            string decodedData;

            try
            {
                decodedData = Encoding.UTF8.GetString(Convert.FromBase64String(data));
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
                    HttpOnly = true,
                    IsEssential = true,
                    MaxAge = TimeSpan.FromMinutes(CookieValidityInMinutes)
                });

            return this.RedirectToAction("Process");
        }

        [HttpGet]
        [Route("/pay")]
        public async Task<IActionResult> Process()
        {
            bool cookieExists = this.Request.Cookies.TryGetValue(PaymentDataCookie, out string data);

            if (!cookieExists)
            {
                return this.BadRequest();
            }

            try
            {
                dynamic deserializedJson = JsonConvert.DeserializeObject(data);

                string paymentInfoJson = deserializedJson.PaymentInfo;

                if (!ValidateSignature(deserializedJson))
                {
                    return this.BadRequest();
                }

                dynamic paymentInfo = JsonConvert.DeserializeObject(paymentInfoJson);

                var banks = (await this.banksService.GetAllBanksSupportingPaymentsAsync<BankListingServiceModel>())
                    .Select(Mapper.Map<BankListingViewModel>)
                    .ToArray();

                var viewModel = new PaymentSelectBankViewModel
                {
                    Amount = paymentInfo.Amount,
                    Description = paymentInfo.Description,
                    Banks = banks
                };

                return this.View(viewModel);
            }
            catch
            {
                return this.BadRequest();
            }
        }

        [HttpPost]
        [Route("/pay/continue")]
        public async Task<IActionResult> Continue([FromForm] string bankId)
        {
            bool cookieExists = this.Request.Cookies.TryGetValue(PaymentDataCookie, out string data);

            if (!cookieExists)
            {
                return this.BadRequest();
            }

            try
            {
                dynamic deserializedJson = JsonConvert.DeserializeObject(data);

                string paymentInfoJson = deserializedJson.PaymentInfo;
                string websitePaymentInfoSignature = deserializedJson.PaymentInfoSignature;
                string returnUrl = deserializedJson.ReturnUrl;

                if (!ValidateSignature(deserializedJson))
                {
                    return this.BadRequest();
                }

                var bank = await this.banksService.GetBankByIdAsync<BankPaymentServiceModel>(bankId);
                if (bank?.PaymentUrl == null)
                {
                    return this.BadRequest();
                }

                // generate PaymentProof containing the bank's public key
                // and merchant's original PaymentInfo signature
                var paymentProof = new
                {
                    BankPublicKey = bank.ApiKey,
                    PaymentInfoSignature = websitePaymentInfoSignature
                };

                string paymentProofJson = JsonConvert.SerializeObject(paymentProof);


                string paymentInfoCentralApiSignature;
                string paymentProofSignature;

                // sign the PaymentInfo and PaymentProof
                using (var centralApiRsa = RSA.Create())
                {
                    RsaExtensions.FromXmlString(centralApiRsa, this.configuration.Key);

                    paymentInfoCentralApiSignature = Convert.ToBase64String(
                        centralApiRsa.SignData(
                            Encoding.UTF8.GetBytes(paymentInfoJson),
                            HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1));

                    paymentProofSignature = Convert.ToBase64String(
                        centralApiRsa.SignData(
                            Encoding.UTF8.GetBytes(paymentProofJson),
                            HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1));
                }

                var toSend = new
                {
                    PaymentInfo = paymentInfoJson,
                    PaymentInfoSignature = paymentInfoCentralApiSignature,
                    PaymentProof = paymentProofJson,
                    PaymentProofSignature = paymentProofSignature,
                    ReturnUrl = returnUrl
                };

                string toSendJson = JsonConvert.SerializeObject(toSend);

                string dataToSend = Convert.ToBase64String(Encoding.UTF8.GetBytes(toSendJson));

                // redirect the user to their bank for payment completion
                var paymentPostRedirectModel = new PaymentPostRedirectModel
                {
                    Url = bank.PaymentUrl,
                    PaymentDataFormKey = PaymentDataFormKey,
                    PaymentData = dataToSend
                };

                return this.View("PaymentPostRedirect", paymentPostRedirectModel);
            }
            catch
            {
                return this.BadRequest();
            }
        }

        private static bool ValidateSignature(dynamic data)
        {
            string paymentInfoJson = data.PaymentInfo;
            string paymentInfoSignature = data.PaymentInfoSignature;
            string websitePublicKey = data.PublicKey;

            // validate PaymentInfo signature to make sure it has not been modified
            // (or at least make it more difficult to modify as it would require signing it with a new key)

            // ! This signature must also be verified by the merchant website after a successful payment
            using (var websiteRsa = RSA.Create())
            {
                RsaExtensions.FromXmlString(websiteRsa, websitePublicKey);

                bool isWebsiteSignatureValid = websiteRsa.VerifyData(
                    Encoding.UTF8.GetBytes(paymentInfoJson),
                    Convert.FromBase64String(paymentInfoSignature),
                    HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

                return isWebsiteSignatureValid;
            }
        }
    }
}