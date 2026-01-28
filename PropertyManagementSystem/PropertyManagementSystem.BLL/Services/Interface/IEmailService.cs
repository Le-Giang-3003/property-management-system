using PropertyManagementSystem.DAL.Entities;

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

        /// <summary>
        /// Sends invoice created notification to tenant.
        /// </summary>
        Task SendInvoiceCreatedToTenantAsync(Invoice invoice, Lease lease);

        /// <summary>
        /// Sends invoice created notification to landlord.
        /// </summary>
        Task SendInvoiceCreatedToLandlordAsync(Invoice invoice, Lease lease);
    }
}
