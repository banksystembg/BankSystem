namespace DemoShop.Services.Models.Order
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class OrderCreateServiceModel
    {
        [Required]
        public string ProductId { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        public DateTime CreatedOn { get; set; }
    }
}