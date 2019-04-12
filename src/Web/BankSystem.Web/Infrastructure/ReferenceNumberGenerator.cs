namespace BankSystem.Web.Infrastructure
{
    using System;

    public static class ReferenceNumberGenerator
    {
        public static string GenerateReferenceNumber()
        {
            var random = new Random();

            var arr = new int[17];
            for (int i = 0; i < 17; i++)
            {
                arr[i] = random.Next(0, 10);
            }

            return string.Join(string.Empty, arr);
        }
    }
}
