namespace BankSystem.Web.Models.BankAccount
{
    using System.ComponentModel.DataAnnotations;
    using Common.AutoMapping.Interfaces;
    using Services.Models.BankAccount;

    public class BankAccountCreateBindingModel : IMapWith<BankAccountCreateServiceModel>
    {
        [MaxLength(50)]
        public string Name { get; set; }
    }
}