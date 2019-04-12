namespace BankSystem.Common
{
    public static class NotificationMessages
    {
        public const string BankAccountCreated = "Bank account created successfully";
        public const string BankAccountCreateError = "An error occured while creating bank account";

        public const string CardCreatedSuccessfully = "Card created successfully";
        public const string CardCreateError = "An error occured while creating card";
        public const string CardDoesNotExist = "Such card does not exist";
        public const string CardDeletedSuccessfully = "Card deleted successfully";
        public const string CardDeleteError = "An error occured while deleting card";

        public const string TryAgainLaterError =
            "Oops! Something went wrong! Please try again later. If this error continues to occur, please contact our support center";

        public const string InsufficientFunds = "Insufficient account balance. Please choose another account.";

        public const string NoAccountsError =
            "Looks like you don't have any accounts. Please feel free to create one and then come back again to process your payment.";

        public const string SuccessfulMoneyTransfer = "Money transfer was successful";

        public const string SameAccountsError =
            "The account which you are sending money from and the destination account must be different";

        public const string AccountDoesNotExist = "Account does not exist";

        public const string DestinationBankAccountDoesNotExist = "The specified destination account does not exist";

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

        public const string SuccessfulRegistration =
                   "Thank you for your registration. To activate your account, please follow the link in the email we have sent you";

        public const string EmailVerificationLinkResentSuccessfully =
            "Account activation link sent successfully";

        public const string SuccessfulEmailVerification =
            "Account activated. You can now log in.";

        public const string EmailVerificationFailed =
            "An error occured while verifying your email. Please try again later and if this error continues to occur, contact our support center";

        public const string EmailAlreadyVerified = "Your email is already verified. You can log in.";

        public const string EmailVerificationRequired =
            "Your account is not activated. If you have not received the activation email, you can request a new one on this page.";
    }
}