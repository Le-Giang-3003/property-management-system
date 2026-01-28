using Microsoft.Extensions.Options;
using PropertyManagementSystem.BLL.Identity;
using PropertyManagementSystem.BLL.Services.Interface;
using System.Net.Mail;

namespace PropertyManagementSystem.BLL.Services.Implementation
{
    public class EmailService : IEmailService
    {
        /// <summary>
        /// The email settings
        /// </summary>
        private readonly EmailSettings _emailSettings;
        /// <summary>
        /// Initializes a new instance of the <see cref="EmailService"/> class.
        /// </summary>
        /// <param name="emailsettings">The emailsettings.</param>
        public EmailService(IOptions<EmailSettings> emailsettings)
        {
            _emailSettings = emailsettings.Value;
        }
        /// <summary>
        /// Sends the email asynchronous.
        /// </summary>
        /// <param name="toEmail">To email.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="body">The body.</param>
        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var email = new MailMessage()
            {
                From = new MailAddress(_emailSettings.SenderEmail, _emailSettings.SenderName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
            };
            email.To.Add(toEmail);
            var stmp = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.SmtpPort)
            {
                Credentials = new System.Net.NetworkCredential(_emailSettings.SenderEmail, _emailSettings.Password),
                EnableSsl = true,
            };
            await stmp.SendMailAsync(email);
        }
    }
}
