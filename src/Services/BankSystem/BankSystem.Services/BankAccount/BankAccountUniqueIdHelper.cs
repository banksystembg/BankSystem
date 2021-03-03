namespace BankSystem.Services.BankAccount
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using Common.Configuration;
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
            => this.bankConfiguration = bankConfigurationOptions.Value;

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

            generated[3] = GenerateCheckCharacter(generated);

            return string.Join("", generated);
        }

        public bool IsUniqueIdValid(string id)
        {
            var rgx = new Regex($@"^{this.bankConfiguration.UniqueIdentifier}[A-Z]\d{{8}}$");

            if (!rgx.IsMatch(id))
            {
                return false;
            }

            char expectedCheckChar = GenerateCheckCharacter(id.ToCharArray());
            char actualCheckChar = id[3];

            return actualCheckChar == expectedCheckChar;
        }

        private static char GenerateCheckCharacter(IReadOnlyList<char> uniqueId)
        {
            int sum = 0;
            for (int i = 0; i < uniqueId.Count; i++)
            {
                if (i == 3)
                {
                    continue;
                }

                sum += (i + 1) * uniqueId[i];
            }

            return (char) ('A' + sum % 26);
        }
    }
}