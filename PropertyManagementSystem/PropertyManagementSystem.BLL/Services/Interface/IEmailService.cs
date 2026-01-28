namespace PropertyManagementSystem.BLL.Services.Interface
{
    /// <summary>
    /// Interface for email service.
    /// </summary>
    public interface IEmailService
    {
        /// <summary>
        /// Sends the email asynchronous.
        /// </summary>
        /// <param name="toEmail">To email.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="body">The body.</param>
        /// <returns></returns>
        Task SendEmailAsync(string toEmail, string subject, string body);
    }
}
