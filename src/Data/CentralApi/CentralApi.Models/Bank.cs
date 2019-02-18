namespace CentralApi.Models
{
    using System.ComponentModel.DataAnnotations;

    public class Bank
    {
        public string Id { get; set; }

        [Required]
        public string ShortName { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string SwiftCode { get; set; }

        [Required]
        public string Location { get; set; }

        [Required]
        public string AppId { get; set; }

        [Required]
        public string ApiKey { get; set; }
    }
}
