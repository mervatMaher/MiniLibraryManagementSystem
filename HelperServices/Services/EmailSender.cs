using MiniLibraryManagementSystem.Helper;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace MiniLibraryManagementSystem.HelperServices.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly SendGridApiAuth _sendGrid;

        public EmailSender(IOptions<SendGridApiAuth> sendGrid)
        {
            _sendGrid = sendGrid.Value;
            Console.WriteLine($"SendGrid API Key: {_sendGrid.SendGridAPiKey}");
        }
        public async Task SendEmailAsync(string tomail, string subject, string Message)
        {
            if (string.IsNullOrEmpty(_sendGrid.SendGridAPiKey))
            {
                throw new Exception("Null SendGridKey");
            }
            await Execute(_sendGrid.SendGridAPiKey, tomail, subject, Message);

        }

        public async Task Execute(string apiKey, string tomail, string subject, string message)
        {
            var client = new SendGridClient(apiKey);
            var sendGridMessage = new SendGridMessage()
            {
                From = new EmailAddress("mervatmaher001@gmail.com"),
                Subject = subject,
                PlainTextContent = message,
                HtmlContent = message,
            };
            sendGridMessage.AddTo(new EmailAddress(tomail));

            sendGridMessage.SetClickTracking(false, false);
            try
            {
                await client.SendEmailAsync(sendGridMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
            }


            //await client.SendEmailAsync(sendGridMessage);
        }
    }
}
