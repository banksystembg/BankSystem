namespace BankSystem.Services.Card
{
    public interface ICardHelper
    {
        bool CheckLuhn(string creditCardNumber);

        string Generate16DigitNumber();

        string Generate3DigitSecurityCode();
    }
}