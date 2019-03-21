namespace BankSystem.Services.Interfaces
{
    public interface ICardHelper
    {
        bool CheckLuhn(string creditCardNumber);

        string Generate16DigitNumber();

        int Generate3DigitSecurityCode();
    }
}
