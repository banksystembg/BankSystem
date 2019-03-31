namespace DemoShop.Models
{
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Identity;

    public class DemoShopUser : IdentityUser
    {
        public ICollection<Order> Orders { get; set; }
    }
}