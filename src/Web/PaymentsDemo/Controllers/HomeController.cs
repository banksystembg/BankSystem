namespace PaymentsDemo.Controllers
{
    using System.Security.Cryptography;
    using BankSystem.Common.Utils;
    using Microsoft.AspNetCore.Mvc;

    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return this.View();
        }

        // This is not necessary to make payments,
        // but can be used when replacing the default keys
        public IActionResult GenerateRsaKeypair()
        {
            using (var rsa = RSA.Create())
            {
                string rsaString = RsaExtensions.ToXmlString(rsa, true);

                return this.Ok(rsaString);
            }
        }
    }
}