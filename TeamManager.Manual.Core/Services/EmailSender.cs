using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using TeamManager.Manual.Core.Interfaces;

namespace TeamManager.Manual.Core.Services
{
    public class EmailSender : IEmailSender
    {
        internal IConfiguration Configuration { get; set; }
        internal ILogger<EmailSender> Logger { get; set; }

        public EmailSender(IConfiguration config, ILogger<EmailSender> emailSenderLogger)
        {
            Configuration = config;
            Logger = emailSenderLogger;
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

        public async Task SendBillDeletedEmailAsync(string to, string lastName, int amount, DateTime purchaseDate, string url)
        {
            string subject = "Törölt számla";
            string message = $"<h2>Kedves {lastName}!</h2><p>Tájékoztatunk hogy a Green Riders rendszerében <a href=\"{url}\">ezt</a> a számládat egy adminisztrátor törölte.</p><p>Összeg: {amount}</p><p>Vásárlás dátuma: {purchaseDate.ToString("yyyy.MM.dd")}</p>";
            await SendEmailAsync(to, subject, message);
        }

        public async Task SendRaceEntryDeadlineIsComingAsync(string to, string name, int days, params string[] races)
        {
            StringBuilder racesString = new StringBuilder();
            for (int i = 0; i < races.Length; i++)
            {
                racesString.Append($"<p>{races[i]}</>");
            }

            string message = $"<h2>Kedves {name}!</h2><p>Értesítünk, hogy a következő versenyek nevezési határideje {days} napon belül lejár.</p><div>{racesString.ToString()}</div>";
            string subject = "Közelgő nevezési határidők";
            await SendEmailAsync(to, subject, message);
        }

        private async Task SendEmailAsync(string to, string subject, string htmlContent)
        {
            if (!IsValidEmailTo(to))
            {
                Logger.LogWarning($"{subject} e-mail cannot be sent to {to}");
                throw new FormatException($"{to}");
            }

            string apiKey = Configuration.GetValue<string>("SendGridAPIKey");
            if (string.IsNullOrEmpty(apiKey))
            {
                Logger.LogError("SendGridAPIKey can not be found in configuration.");
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
                Logger.LogError($"Email sending failed: {sendGridResponse.StatusCode} {sendGridResponse.Body}");
                throw new Exception();
            }

            Logger.LogInformation($"E-mail sent to {to} about {subject}");
        }

        private bool IsValidEmailTo(string to)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(to))
                    return false;

                MailAddress mailAddress = new MailAddress(to);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }


    }
}
