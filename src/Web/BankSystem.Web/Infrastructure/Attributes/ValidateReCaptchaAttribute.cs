namespace BankSystem.Web.Infrastructure.Attributes
{
    using System.ComponentModel.DataAnnotations;
    using System.Net;
    using System.Net.Http;
    using Microsoft.Extensions.Configuration;
    using Newtonsoft.Json.Linq;

    public class ValidateReCaptchaAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var errorResult = new ValidationResult("reCAPTCHA validation failed. Please try again.",
                new[] {validationContext.MemberName});

            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return errorResult;
            }

            var response = value.ToString();

            var configuration = (IConfiguration) validationContext.GetService(typeof(IConfiguration));
            var secret = configuration?["ReCaptcha:SiteSecret"];

            var httpClient = new HttpClient();
            var httpResponse = httpClient
                .GetAsync($"https://www.google.com/recaptcha/api/siteverify?secret={secret}&response={response}")
                .GetAwaiter().GetResult();

            if (httpResponse.StatusCode != HttpStatusCode.OK)
            {
                return errorResult;
            }

            var jsonResponse = httpResponse.Content.ReadAsStringAsync().Result;
            dynamic jsonData = JObject.Parse(jsonResponse);

            return jsonData.success == "true"
                ? ValidationResult.Success
                : errorResult;
        }
    }
}