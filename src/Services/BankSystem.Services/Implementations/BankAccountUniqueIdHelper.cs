namespace BankSystem.Services.Implementations
{
    using System;
    using System.Text.RegularExpressions;
    using Common.Configuration;
    using Interfaces;
    using Microsoft.Extensions.Options;

    public class BankAccountUniqueIdHelper : IBankAccountUniqueIdHelper
    {
        /* 
         * Sample UniqueId: ABCG12345678
         * 
         * ABC        Unique bank identifier
         * G          Check character
         * 12345678   Random unique number
         * 
         */

        private readonly BankConfiguration bankConfiguration;
        private Random random;

        public BankAccountUniqueIdHelper(IOptions<BankConfiguration> bankConfigurationOptions)
        {
            this.bankConfiguration = bankConfigurationOptions.Value;
        }

        public string GenerateAccountUniqueId()
        {
            if (this.random == null)
            {
                this.random = new Random();
            }

            char[] generated = new char[12];

            for (int i = 0; i < 3; i++)
            {
                generated[i] = this.bankConfiguration.UniqueIdentifier[i];
            }

            for (int i = 0; i < 8; i++)
            {
                generated[i + 4] = (char) ('0' + this.random.Next(10));
            }

            int weightedSum = 0;

            for (int i = 0; i < generated.Length; i++)
            {
                weightedSum += (i + 1) * generated[i];
            }

            generated[3] = (char) ('A' + weightedSum % 26);

            return string.Join("", generated);
        }

        public bool IsUniqueIdValid(string id)
        {
            var rgx = new Regex($@"^{this.bankConfiguration.UniqueIdentifier}[A-Z]\d{{8}}$");

            if (!rgx.IsMatch(id))
            {
                return false;
            }

            int weightedSum = 0;

            for (int i = 0; i < id.Length; i++)
            {
                if (i == 3)
                {
                    continue;
                }

                weightedSum += (i + 1) * id[i];
            }

            char expectedCheckChar = (char) ('A' + weightedSum % 26);

            char actualCheckChar = id[3];

            return actualCheckChar == expectedCheckChar;
        }
    }
}