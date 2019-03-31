namespace BankSystem.Web.Models
{
    using System.ComponentModel.DataAnnotations;
    using Infrastructure.Attributes;
    using Microsoft.AspNetCore.Mvc;

    public abstract class BaseReCaptchaModel
    {
        [Required]
        [ValidateReCaptcha]
        [BindProperty(Name = "g-recaptcha-response")]
        public string ReCaptchaResponse { get; set; }
    }
}