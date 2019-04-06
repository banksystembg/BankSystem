namespace BankSystem.Web.Infrastructure.Helpers
{
    using System;
    using System.Security.Cryptography;
    using System.Text;
    using Common.Utils;
    using Newtonsoft.Json;

    public static class DirectPaymentsHelper
    {
        public static string DecodePaymentRequest(string request)
            => Encoding.UTF8.GetString(Convert.FromBase64String(request));

        public static dynamic ParsePaymentRequest(string jsonRequest, string centralApiPublicKey)
        {
            dynamic paymentRequest = JsonConvert.DeserializeObject(jsonRequest);

            if (!ValidateSignatures(paymentRequest, centralApiPublicKey))
            {
                return null;
            }

            dynamic paymentInfo = GetPaymentInfo(paymentRequest);
            if (paymentInfo.Amount <= 0)
            {
                return null;
            }

            return paymentRequest;
        }

        public static dynamic GetPaymentInfo(dynamic paymentRequest)
            => JsonConvert.DeserializeObject((string) paymentRequest.PaymentInfo);

        public static string GenerateSuccessResponse(dynamic paymentRequest, string bankKey)
        {
            // generate PaymentConfirmation
            var paymentConfirmation = new
            {
                Success = true,
                paymentRequest.PaymentProofSignature
            };

            var paymentConfirmationJson = JsonConvert.SerializeObject(paymentConfirmation);

            // sign the PaymentConfirmation
            string paymentConfirmationSignature;

            using (var bankRsa = RSA.Create())
            {
                RsaExtensions.FromXmlString(bankRsa, bankKey);

                paymentConfirmationSignature = Convert.ToBase64String(
                    bankRsa.SignData(
                        Encoding.UTF8.GetBytes(paymentConfirmationJson),
                        HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1));
            }

            // generate response
            var response = new
            {
                paymentRequest.PaymentInfo,
                paymentRequest.PaymentProof,
                PaymentConfirmation = paymentConfirmationJson,
                PaymentConfirmationSignature = paymentConfirmationSignature
            };

            var responseJson = JsonConvert.SerializeObject(response);

            var base64Response = Convert.ToBase64String(Encoding.UTF8.GetBytes(responseJson));

            return base64Response;
        }

        public static string Sha256Hash(string data)
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

        private static bool ValidateSignatures(dynamic data, string centralApiPublicKey)
        {
            string paymentInfoJson = data.PaymentInfo;
            string centralApiPaymentInfoSignature = data.PaymentInfoSignature;
            string paymentProofJson = data.PaymentProof;
            string paymentProofSignature = data.PaymentProofSignature;

            // validate signatures of PaymentInfo and PaymentProof
            using (var centralApiRsa = RSA.Create())
            {
                RsaExtensions.FromXmlString(centralApiRsa, centralApiPublicKey);

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
    }
}