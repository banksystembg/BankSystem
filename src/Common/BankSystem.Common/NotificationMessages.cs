namespace BankSystem.Common
{
    public static class NotificationMessages
    {
        public const string BankAccountCreated = "Bank account created successfully";
        public const string BankAccountCreateError = "An error occured while creating bank account";

        public const string TryAgainLaterError =
            "Oops! Something went wrong! Please try again later. If this error continues to occur, please contact our support center";

        public const string InsufficientFunds =
            "Insufficient account balance. Please deposit money in your account and come back to process the payment or choose another account";

        public const string NoAccountsError =
            "Looks like you don't have any accounts. Please feel free to create one and then come back again to process your payment.";

        public const string SuccessfulMoneyTransfer = "Money transfer was successful";

        public const string SameAccountsError =
            "The account from which you are sending money and the destination account must be different!";

        public const string DestinationBankAccountDoesNotExist = "Destination bank account does not exist";

        public const string InvalidCredentials = "Invalid email or password";
        public const string PasswordChangeSuccessful = "Password changed successfully";

        public const string InvalidPassword = "Invalid password";

        public const string LoginLockedOut =
            "Your account is locked because of too many invalid login attempts. Please try again in a few minutes.";

        public const string TwoFactorAuthenticationCodeInvalid = "Invalid code";
        public const string TwoFactorAuthenticationEnabled = "Two-factor authentication enabled successfully";
        public const string TwoFactorAuthenticationDisabled = "Two-factor authentication disabled successfully";

        public const string TwoFactorAuthenticationDisableError =
            "An error occured while disabling two-factor authentication";

        public const string SessionExpired = "Session has expired. Please log in again.";

        public const string PaymentStateInvalid =
            "Payment details are invalid or have expired. Please try again later.";

        public const string PaymentFailed = "Payment failed. Please try again later.";
    }
}