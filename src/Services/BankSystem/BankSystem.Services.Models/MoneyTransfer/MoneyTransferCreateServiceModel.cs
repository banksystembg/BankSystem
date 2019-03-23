namespace BankSystem.Services.Models.MoneyTransfer
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using AutoMapper;
    using BankSystem.Models;
    using Common.AutoMapping.Interfaces;

    public class MoneyTransferCreateServiceModel : MoneyTransferBaseServiceModel, IHaveCustomMapping
    {
        public string Description { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public string AccountId { get; set; }

        [Required]
        public string SenderName { get; set; }

        [Required]
        public string RecipientName { get; set; }

        [Required]
        public DateTime MadeOn { get; set; } = DateTime.UtcNow;

        [Required]
        public string Source { get; set; }

        [Required]
        public string DestinationBankAccountUniqueId { get; set; }

        public void ConfigureMapping(Profile mapper)
        {
            mapper.CreateMap<MoneyTransferCreateServiceModel, MoneyTransfer>()
                .ForMember(dest => dest.Destination, opt => opt.MapFrom(src => src.DestinationBankAccountUniqueId));
        }
    }
}
