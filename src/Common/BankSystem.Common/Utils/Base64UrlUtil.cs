namespace BankSystem.Common.Utils
{
    using System;
    using System.Text;

    public static class Base64UrlUtil
    {
        /* Characters replaced for URL compatibility:
         *
         * + -> .
         * / -> _
         * = -> -
         *
         */

        public static string Decode(string base64)
        {
            base64 = base64
                .Replace('.', '+')
                .Replace('_', '/')
                .Replace('-', '=');

            return Encoding.UTF8.GetString(
                Convert.FromBase64String(base64));
        }

        public static string Encode(string data)
        {
            string base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(data));

            base64 = base64
                .Replace('+', '.')
                .Replace('/', '_')
                .Replace('=', '-');

            return base64;
        }
    }
}