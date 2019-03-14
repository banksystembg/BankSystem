namespace BankSystem.Services.Implementations
{
    using Common.Configuration;
    using Interfaces;
    using Microsoft.Extensions.Options;

    public class BankConfigurationHelper : IBankConfigurationHelper
    {
        private readonly BankConfiguration bankConfiguration;

        public BankConfigurationHelper(IOptions<BankConfiguration> bankConfigurationOptions)
        {
            this.bankConfiguration = bankConfigurationOptions.Value;
        }

        public string UniqueIdentifier => this.bankConfiguration.UniqueIdentifier;
        public string Key => this.bankConfiguration.Key;
        public string CentralApiPublicKey => this.bankConfiguration.CentralApiPublicKey;
        public string First3CardDigits => this.bankConfiguration.First3CardDigits;
    }
}
