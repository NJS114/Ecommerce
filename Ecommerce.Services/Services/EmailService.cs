using Ecommerce.Services.DAO.Interfaces.IServices;
using MailKit.Net.Smtp;
using MimeKit;

namespace Ecommerce.Services.Services
{
    public class EmailService: IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly EmailService _emailService;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {

            var smtpServer = Environment.GetEnvironmentVariable("EMAIL_SETTINGS_SMTP_SERVER");
            var smtpPort = int.Parse(Environment.GetEnvironmentVariable("EMAIL_SETTINGS_PORT"));
            var smtpUsername = Environment.GetEnvironmentVariable("EMAIL_SETTINGS_USERNAME");
            var smtpPassword = Environment.GetEnvironmentVariable("EMAIL_SETTINGS_PASSWORD");
            var smtpFromEmail = Environment.GetEnvironmentVariable("EMAIL_SETTINGS_FROM_EMAIL");

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Lectio", smtpFromEmail));
            message.To.Add(new MailboxAddress("", toEmail));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder { HtmlBody = body };
            message.Body = bodyBuilder.ToMessageBody();

            try
            {
                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync(smtpServer, smtpPort, MailKit.Security.SecureSocketOptions.StartTls);

                    await client.AuthenticateAsync(smtpUsername, smtpPassword);

                    await client.SendAsync(message);

                    await client.DisconnectAsync(true);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de l'envoi de l'email: {ex.Message}");
                throw;
            }
        }
    }
}
