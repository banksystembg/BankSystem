namespace BankSystem.Services.Implementations
{
    using Common.Configuration;
    using Interfaces;
    using Microsoft.Extensions.Options;

    public class BankConfigurationService : IBankConfigurationService
    {
        private readonly BankConfiguration bankConfiguration;

        public BankConfigurationService(IOptions<BankConfiguration> bankConfigurationOptions)
        {
            this.bankConfiguration = bankConfigurationOptions.Value;
        }

        public string UniqueIdentifier => this.bankConfiguration.UniqueIdentifier;
        public string Key => this.bankConfiguration.Key;
    }
}
