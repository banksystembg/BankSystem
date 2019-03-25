namespace BankSystem.Common
{
    public static class GlobalConstants
    {
        public const string TempDataErrorMessageKey = "ErrorMessage";
        public const string TempDataSuccessMessageKey = "SuccessMessage";

        public const string TempDataNoTwoFactorKey = "2FANotEnabled";
        public const string IgnoreTwoFactorWarningCookie = "IgnoreTwoFactorWarning";

        public const string CardExpirationDateFormat = "MM/yy";
        public const int CardValidityInYears = 4;

        public const string AuthorizationHeader = "Authorization";
        public const string AuthenticationScheme = "bsw";
    }
}