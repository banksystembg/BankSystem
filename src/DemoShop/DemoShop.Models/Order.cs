namespace DemoShop.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class Order
    {
        public string Id { get; set; }

        [Required]
        public string ProductId { get; set; }

        public Product Product { get; set; }

        [Required]
        public string UserId { get; set; }

        public DemoShopUser User { get; set; }

        [Required]
        public PaymentStatus PaymentStatus { get; set; }

        [Required]
        public DateTime CreatedOn { get; set; }
    }
}