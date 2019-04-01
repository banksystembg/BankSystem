namespace DemoShop.Web.PaymentHelpers
{
    using System;
    using System.Security.Cryptography;
    using System.Text;
    using Newtonsoft.Json;

    public static class DirectPaymentsHelper
    {
        public static string GeneratePaymentRequest(object paymentInfo, string siteKey, string returnUrl)
        {
            var paymentInfoJson = JsonConvert.SerializeObject(paymentInfo);

            string paymentInfoSignature;
            string websitePublicKey;

            // sign the PaymentInfo for verification when receiving the payment result
            // and include the public key of the merchant website for signature verification

            // the public key and signature could be modified by an attacker,
            // but this will be detected when receiving the payment result
            using (var websiteRsa = RSA.Create())
            {
                RsaExtensions.FromXmlString(websiteRsa, siteKey);

                paymentInfoSignature = Convert.ToBase64String(
                    websiteRsa.SignData(
                        Encoding.UTF8.GetBytes(paymentInfoJson),
                        HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1));

                websitePublicKey = RsaExtensions.ToXmlString(websiteRsa, false);
            }

            var paymentRequest = new
            {
                PaymentInfo = paymentInfoJson,
                PaymentInfoSignature = paymentInfoSignature,
                PublicKey = websitePublicKey,
                ReturnUrl = returnUrl
            };

            string paymentRequestJson = JsonConvert.SerializeObject(paymentRequest);

            string data = Convert.ToBase64String(Encoding.UTF8.GetBytes(paymentRequestJson));

            return data;
        }

        public static dynamic ProcessPaymentResult(string data, string siteKey, string centralApiKey)
        {
            try
            {
                string decodedJson = Encoding.UTF8.GetString(Convert.FromBase64String(data));

                // deserialize json
                dynamic deserializedJson = JsonConvert.DeserializeObject(decodedJson);

                string paymentInfoJson = deserializedJson.PaymentInfo;
                string paymentProofJson = deserializedJson.PaymentProof;
                string paymentConfirmationJson = deserializedJson.PaymentConfirmation;
                string bankPaymentConfirmationSignature = deserializedJson.PaymentConfirmationSignature;

                dynamic paymentInfo = JsonConvert.DeserializeObject(paymentInfoJson);
                dynamic paymentProof = JsonConvert.DeserializeObject(paymentProofJson);
                dynamic paymentConfirmation = JsonConvert.DeserializeObject(paymentConfirmationJson);

                string bankPublicKey = paymentProof.BankPublicKey;
                string paymentInfoSignature = paymentProof.PaymentInfoSignature;

                string paymentProofSignature = paymentConfirmation.PaymentProofSignature;
                bool success = paymentConfirmation.Success;


                // Verify the payment confirmation trust chain
                // (the central api provides us with the bank's key)

                // verify the PaymentInfo to make sure it has not been modified before being processed by the bank
                bool isPaymentInfoSignatureValid = VerifySignature(paymentInfoJson,
                    paymentInfoSignature, siteKey);

                if (!isPaymentInfoSignatureValid)
                {
                    return null;
                }

                // verify the PaymentProof to make sure the provided bank key is trusted by the central api
                bool isPaymentProofSignatureValid = VerifySignature(paymentProofJson,
                    paymentProofSignature, centralApiKey);

                if (!isPaymentProofSignatureValid)
                {
                    return null;
                }

                // finally, verify the PaymentConfirmation using the (now trusted) bank's key
                bool isPaymentConfirmationSignatureValid = VerifySignature(paymentConfirmationJson,
                    bankPaymentConfirmationSignature, bankPublicKey);

                if (!isPaymentConfirmationSignatureValid)
                {
                    return null;
                }

                // check if the payment is successfully completed
                if (success != true)
                {
                    return null;
                }

                // return the decoded and verified PaymentInfo
                return paymentInfo;
            }
            catch
            {
                return null;
            }
        }

        private static bool VerifySignature(string data, string signature, string key)
        {
            using (var rsa = RSA.Create())
            {
                RsaExtensions.FromXmlString(rsa, key);

                bool isSignatureValid = rsa.VerifyData(
                    Encoding.UTF8.GetBytes(data),
                    Convert.FromBase64String(signature),
                    HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

                return isSignatureValid;
            }
        }
    }
}