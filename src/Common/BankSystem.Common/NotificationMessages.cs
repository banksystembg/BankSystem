namespace BankSystem.Common
{
    public static class NotificationMessages
    {
        public const string BankAccountCreated = "Bank account created successfully";
        public const string BankAccountCreateError = "An error occured while creating bank account";
        public const string TryAgainLaterError =
            "Oops! Something went wrong! Please try again later. If this error continues to occur, please contact our support center";

        public const string NoAccountsError =
            "Looks like you don't have any accounts. Please feel free to create one and then come back again to process your payment.";

        public const string SuccessfulMoneyTransfer = "Money transfer was successful";
    }
}