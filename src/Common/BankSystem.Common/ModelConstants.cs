namespace BankSystem.Common
{
    public static class ModelConstants
    {
        public static class User
        {
            public const int FullNameMaxLength = 50;

            public const int PasswordMaxLength = 100;
            public const int PasswordMinLength = 6;

            public const string PasswordRegex = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{6,100}$";

            public const string PasswordErrorMessage =
                "Password minimum length is 6 characters and it must contain at least one uppercase letter, one lowercase letter and one number.";
        }

        public static class BankAccount
        {
            public const int NameMaxLength = 35;
            public const int UniqueIdMaxLength = 34;
            public const int SwiftCodeMaxLength = 11;
            public const int CountryMaxLength = 35;
        }

        public static class Card
        {
            public const int NameMaxLength = 50;

            public const int ExpiryDateMaxLength = 5;
            public const int SecurityCodeMaxLength = 3;
        }

        public static class MoneyTransfer
        {
            public const int DescriptionMaxLength = 150;
            public const string MinStartingPrice = "0.01";
            public const string MaxStartingPrice = "79228162514264337593543950335";
        }
    }
}