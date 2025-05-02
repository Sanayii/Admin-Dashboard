using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Logging;

namespace Admin_Dashboard.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _config;
        private readonly ILogger<EmailSender> _logger;

        public EmailSender(IConfiguration config, ILogger<EmailSender> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            try
            {
                var emailSettings = _config.GetSection("EmailSettings");
                var smtpServer = emailSettings["SmtpServer"];
                var port = int.Parse(emailSettings["Port"]);
                var username = emailSettings["Username"];
                var password = emailSettings["Password"];

                using var client = new SmtpClient(smtpServer, port)
                {
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(username, password),
                    Timeout = 10000 // 10 ثواني
                };

                var mail = new MailMessage
                {
                    From = new MailAddress(username, "Sanayii"),
                    Subject = subject,
                    Body = htmlMessage,
                    IsBodyHtml = true
                };
                mail.To.Add(email);

                await client.SendMailAsync(mail);
                _logger.LogInformation($"Email sent to {email} successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to send email to {email}: {ex}");
                throw;
            }
        }
    }
}