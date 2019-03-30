namespace DemoShop.Web.Models
{
    using System.ComponentModel.DataAnnotations;

    public class CardPaymentBindingModel
    {
        [Required]
        [Display(Name = "Card number")]
        [StringLength(16, MinimumLength = 16)]
        [RegularExpression(@"^\d{16}$", ErrorMessage = "Invalid card number")]
        [CreditCard]
        public string Number { get; set; }

        [Required]
        [Display(Name = "Cardholder name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Expiration date")]
        [StringLength(5, MinimumLength = 5)]
        [RegularExpression(@"^(0[1-9]|1[0-c2])\/\d{2}$", ErrorMessage = "Invalid expiration date")]
        public string ExpiryDate { get; set; }

        [Required]
        [Display(Name = "Security code")]
        [StringLength(3, MinimumLength = 3)]
        [RegularExpression(@"\d{3}", ErrorMessage = "Invalid security code")]
        public string SecurityCode { get; set; }

        public string ProductName { get; set; }

        public decimal ProductPrice { get; set; }
    }
}