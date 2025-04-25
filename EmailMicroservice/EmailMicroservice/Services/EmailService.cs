using EmailMicroservice.Models.Configurations;
using EmailMicroservice.Services.Interfaces;
using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Net;

namespace EmailMicroservice.Services
{
    public class EmailService : IEmailService
    {
        private readonly SmtpSettings settings;

        public EmailService(IOptions<SmtpSettings> smtpSettings)
        {
            settings = smtpSettings.Value;
        }
        public async Task<(bool Success, string? message)> SendEmailAsync(string email, string message)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return (false, "Email не может быть пустым");
            }

            email = email.Trim();


            try
            {
                using var smtpClient = new SmtpClient(settings.Server, settings.Port)
                {
                    Credentials = new NetworkCredential(settings.Username, settings.Password),
                    EnableSsl = true
                };

                using var mailMessage = new MailMessage
                {
                    From = new MailAddress(settings.Username),
                    Subject = "Сообщение из микросервиса",
                    Body = message,
                    IsBodyHtml = false
                };

                mailMessage.To.Add(email);

                await smtpClient.SendMailAsync(mailMessage);
                return (true, null);
            }
            catch (SmtpException ex)
            {
                return (false, $"Ошибка SMTP: {ex.StatusCode}");
            }
            catch (Exception ex)
            {
                return (false, $"Ошибка отправки: {ex.Message}");
            }
        }
    }
}
