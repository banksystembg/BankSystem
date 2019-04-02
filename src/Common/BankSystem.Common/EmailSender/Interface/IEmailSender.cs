namespace BankSystem.Common.EmailSender.Interface
{
    using System.Threading.Tasks;

    public interface IEmailSender
    {
        Task<bool> SendEmailAsync(string receiver, string subject, string htmlMessage);
        Task<bool> SendEmailAsync(string sender, string receiver, string subject, string htmlMessage);
    }
}