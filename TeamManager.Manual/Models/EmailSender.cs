using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;
using TeamManager.Manual.Models.Exceptions;
using TeamManager.Manual.Models.Interfaces;

namespace TeamManager.Manual.Models
{
    public class EmailSender : IEmailSender
    {
        private IConfiguration configuration;

        public EmailSender(IConfiguration config)
        {
            configuration = config;
        }

        public async Task SendAdminVerifiedEmailAsync(string to, string firstName, string loginAddress)
        {
            await SendEmailAsync(to, "Green Riders Team Manager - Admin jóváhagyás", $"<h3>Kedves {firstName}! Az adminisztrátorok jóváhagyták regisztrációdat a Green Riders rendszerébe, most már be tudsz lépni. <a href=\"{loginAddress}\">Belépek</a>");
        }

        public async Task SendContactEmailAsync(string to, string message, string replyTo)
        {
            await SendEmailAsync(to, $"Team Manager - Contact", $"<p>Feladó: <a href=mailto:{replyTo}>{replyTo}</a></p><p>Üzenet: {message}</p>");
        }

        public async Task SendForgotPasswordEmailAsync(string to, string name, string token, string userId, string baseUrl)
        {
            string subject = "Elfelejtett jelszó";
            string message = $"<h2>Kedves {name}!</h2> <p>Új jelszó beállításához a Green Riders Team Management rendszerében kattints <a href=\"{baseUrl}/Account/ResetPassword?token={token}&userId={userId}\">ide</a>.</p>";
            await SendEmailAsync(to, subject, message);
        }

        private async Task SendEmailAsync(string to, string subject, string htmlContent)
        {
            string apiKey = configuration.GetValue<string>("SendGridAPIKey");
            if (string.IsNullOrEmpty(apiKey))
            {
                return;
            }

            SendGridClient client = new SendGridClient(apiKey);
            SendGridMessage message = new SendGridMessage();
            message.AddTo(to);
            message.Subject = subject;
            message.HtmlContent = htmlContent;
            message.From = new EmailAddress("greenridershu@gmail.com", "Green Riders Team Manager");

            Response sendGridResponse = await client.SendEmailAsync(message);
            if (sendGridResponse.StatusCode != System.Net.HttpStatusCode.Accepted)
            {
                throw new EmailSendingException();
            }
        }
    }
}
