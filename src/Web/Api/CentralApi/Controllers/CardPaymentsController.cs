namespace CentralApi.Controllers
{
    using BankSystem.Common.Utils;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using Models;
    using Newtonsoft.Json;
    using Services.Interfaces;
    using Services.Models.Banks;
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading.Tasks;

    [ApiController]
    [Route("api/[controller]")]
    public class CardPaymentsController : ControllerBase
    {
        private readonly IBanksService banksService;
        private readonly CentralApiConfiguration configuration;

        public CardPaymentsController(IBanksService banksService, IOptions<CentralApiConfiguration> configuration)
        {
            this.banksService = banksService;
            this.configuration = configuration.Value;
        }

        [HttpPost("{data}", Name = "Post")]
        public async Task<IActionResult> Process([FromBody] string data)
        {
            try
            {
                data = Encoding.UTF8.GetString(Convert.FromBase64String(data));
                var model = JsonConvert.DeserializeObject<CardPaymentReceiveModel>(data);

                var payment = JsonConvert.DeserializeObject<CardPaymentDto>(model.PaymentInfo);
                var websitePaymentInfoSignature = model.PaymentInfoSignature;

                if (!ValidateSignature(model))
                {
                    return this.BadRequest();
                }

                var first3Digits = payment.Number.Substring(0, 3);
                var bank = await this.banksService
                    .GetBankByBankIdentificationCardNumbersAsync<BankPaymentServiceModel>(first3Digits);
                if (bank?.CardPaymentUrl == null)
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

                var paymentProofJson = JsonConvert.SerializeObject(paymentProof);


                string paymentInfoCentralApiSignature;
                string paymentProofSignature;

                // sign the PaymentInfo and PaymentProof
                using (var centralApiRsa = RSA.Create())
                {
                    RsaExtensions.FromXmlString(centralApiRsa, this.configuration.Key);

                    paymentInfoCentralApiSignature = Convert.ToBase64String(
                        centralApiRsa.SignData(
                            Encoding.UTF8.GetBytes(model.PaymentInfo),
                            HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1));

                    paymentProofSignature = Convert.ToBase64String(
                        centralApiRsa.SignData(
                            Encoding.UTF8.GetBytes(paymentProofJson),
                            HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1));
                }

                var toSend = new
                {
                    model.PaymentInfo,
                    PaymentInfoSignature = paymentInfoCentralApiSignature,
                    PaymentProof = paymentProofJson,
                    PaymentProofSignature = paymentProofSignature,
                };

                var toSendJson = JsonConvert.SerializeObject(toSend);
                var dataToSend = Convert.ToBase64String(Encoding.UTF8.GetBytes(toSendJson));

                var client = new HttpClient();
                var request = await client.PostAsJsonAsync(bank.CardPaymentUrl, dataToSend);
                var responseContent = request.Content.ReadAsStringAsync();

                if (request.StatusCode != HttpStatusCode.OK)
                {
                    return this.BadRequest(responseContent);
                }

                return this.Ok(responseContent);
            }
            catch
            {
                return this.BadRequest();
            }
        }

        private static bool ValidateSignature(CardPaymentReceiveModel data)
        {
            var paymentInfoJson = data.PaymentInfo;
            var paymentInfoSignature = data.PaymentInfoSignature;
            var websitePublicKey = data.PublicKey;

            // validate PaymentInfo signature to make sure it has not been modified
            // (or at least make it more difficult to modify as it would require signing it with a new key)

            // ! This signature must also be verified by the merchant website after a successful payment
            using (var websiteRsa = RSA.Create())
            {
                RsaExtensions.FromXmlString(websiteRsa, websitePublicKey);

                var isWebsiteSignatureValid = websiteRsa.VerifyData(
                    Encoding.UTF8.GetBytes(paymentInfoJson),
                    Convert.FromBase64String(paymentInfoSignature),
                    HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

                return isWebsiteSignatureValid;
            }
        }
    }
}
