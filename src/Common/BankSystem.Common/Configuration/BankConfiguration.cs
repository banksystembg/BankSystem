namespace BankSystem.Common.Configuration
{
    using System.ComponentModel.DataAnnotations;

    public class BankConfiguration
    {
        [Required]
        [RegularExpression(@"^[A-Z]{3}$")]
        public string UniqueIdentifier { get; set; }

        [Required]
        public string Key { get; set; }
    }
}