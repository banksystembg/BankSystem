namespace CentralApi.Controllers
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading.Tasks;
    using AutoMapper;
    using BankSystem.Common.Utils;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using Models;
    using Newtonsoft.Json;
    using Services.Interfaces;
    using Services.Models.Banks;

    public class PaymentsController : Controller
    {
        private const int LinkValidityInMinutes = 5;
        private const string DateFormat = "yyyyMMddHHmmss";

        private readonly IBanksService banksService;
        private readonly CentralApiConfiguration configuration;

        public PaymentsController(IBanksService banksService, IOptions<CentralApiConfiguration> configuration)
        {
            this.banksService = banksService;
            this.configuration = configuration.Value;
        }

        [HttpGet]
        [Route("/pay/{data}")]
        public async Task<IActionResult> Process(string data, string expire)
        {
            // Append expiration date to URL to prevent accidental duplicate payments
            if (expire == null)
            {
                expire = DateTime.UtcNow.AddMinutes(LinkValidityInMinutes).ToString(DateFormat);

                return this.RedirectToAction("Process", new {data, expire});
            }

            bool dateParsed = DateTime.TryParseExact(expire, DateFormat, null, DateTimeStyles.None,
                out var expirationDate);

            if (!dateParsed || expirationDate < DateTime.UtcNow)
            {
                return this.BadRequest();
            }

            try
            {
                dynamic deserializedJson = JsonConvert.DeserializeObject(Base64UrlUtil.Decode(data));

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
        [Route("/pay/{data}")]
        public async Task<IActionResult> Process(string data, string expire, [FromForm] string bankId)
        {
            if (data == null || expire == null || bankId == null)
            {
                return this.BadRequest();
            }

            // Verify that the link has not expired
            bool dateParsed = DateTime.TryParseExact(expire, DateFormat, null, DateTimeStyles.None,
                out var expirationDate);

            if (!dateParsed || expirationDate < DateTime.UtcNow)
            {
                return this.BadRequest();
            }

            try
            {
                dynamic deserializedJson = JsonConvert.DeserializeObject(Base64UrlUtil.Decode(data));

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

                string dataToSend = Base64UrlUtil.Encode(toSendJson);

                return this.Redirect(string.Format(bank.PaymentUrl, dataToSend));
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