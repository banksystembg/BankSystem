namespace BankSystem.Common
{
    public static class EmailMessages
    {
        public const string ConfirmEmailSubject = "BankSystem email confirmation";
        public const string EmailConfirmationMessage = "Please confirm your email by <a href=\"{0}\">clicking here</a>.";
        public const string EmailConfirmationPage = "/Account/ConfirmEmail";

        public const string ReceiveMoneySubject = "You've received money";
        public const string ReceiveMoneyMessage = "€{0} have been transferred to your account. Please log in your account for additional information.";
        public const string SendMoneySubject = "You've sent money";

        public const string SendMoneyMessage =
            "€{0} have been transferred from your account. If it was not you, please contact our support center as fast as possible!";
    }
}
