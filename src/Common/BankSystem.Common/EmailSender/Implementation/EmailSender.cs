namespace BankSystem.Common.EmailSender.Implementation
{
    using System.Net;
    using System.Threading.Tasks;
    using Interface;
    using Microsoft.Extensions.Options;
    using SendGrid;
    using SendGrid.Helpers.Mail;

    public class EmailSender : IEmailSender
    {
        private readonly SendGridConfiguration options;

        public EmailSender(IOptions<SendGridConfiguration> options)
            => this.options = options.Value;

        public async Task<bool> SendEmailAsync(string receiver, string subject, string htmlMessage)
            => await this.SendEmailAsync(GlobalConstants.BankSystemEmail, receiver, subject, htmlMessage);

        public async Task<bool> SendEmailAsync(string sender, string receiver, string subject, string htmlMessage)
        {
            var client = new SendGridClient(this.options.ApiKey);
            var from = new EmailAddress(sender);
            var to = new EmailAddress(receiver, receiver);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, htmlMessage, htmlMessage);

            try
            {
                var isSuccessful = await client.SendEmailAsync(msg);

                return isSuccessful.StatusCode == HttpStatusCode.Accepted;
            }
            catch
            {
                // Ignored
                return false;
            }
        }
    }
}