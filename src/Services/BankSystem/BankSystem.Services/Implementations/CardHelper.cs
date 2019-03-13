namespace BankSystem.Services.Implementations
{
    using Interfaces;
    using System;
    using System.Text;

    public class CardHelper : ICardHelper
    {
        private readonly Random random;

        public CardHelper()
        {
            this.random = new Random();
        }


        public long Generate16DigitNumber()
        {
            var stringBuilder = new StringBuilder();
            while (stringBuilder.Length < 16)
            {
                stringBuilder.Append(this.random.Next(9).ToString());
            }

            return long.Parse(stringBuilder.ToString());
        }

        public int Generate3DigitSecurityCode()
        {
            var stringBuilder = new StringBuilder();
            while (stringBuilder.Length < 3)
            {
                stringBuilder.Append(this.random.Next(10).ToString());
            }

            return int.Parse(stringBuilder.ToString());
        }
    }
}
