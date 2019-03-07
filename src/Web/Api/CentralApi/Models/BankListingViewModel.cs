namespace CentralApi.Models
{
    using BankSystem.Common.AutoMapping.Interfaces;
    using Services.Models.Banks;

    public class BankListingViewModel : IMapWith<BankListingServiceModel>
    {
        public string Name { get; set; }

        public string Location { get; set; }

        public string SwiftCode { get; set; }

        public string Id { get; set; }
    }
}