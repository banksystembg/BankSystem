namespace BankSystem.Services.Interfaces
{
    public interface ICardHelper
    {
        long Generate16DigitNumber();

        int Generate3DigitSecurityCode();
    }
}
