namespace CentralApi
{
    using System.ComponentModel.DataAnnotations;

    public class CentralApiConfiguration
    {
        [Required]
        public string Key { get; set; }
    }
}