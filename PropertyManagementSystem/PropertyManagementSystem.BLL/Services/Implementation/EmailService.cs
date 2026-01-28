using Microsoft.Extensions.Options;
using PropertyManagementSystem.BLL.Identity;
using PropertyManagementSystem.BLL.Services.Interface;
using PropertyManagementSystem.DAL.Entities;
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

        /// <summary>
        /// Sends invoice created notification to tenant.
        /// </summary>
        public async Task SendInvoiceCreatedToTenantAsync(Invoice invoice, Lease lease)
        {
            var tenantEmail = lease.Tenant?.Email;
            if (string.IsNullOrEmpty(tenantEmail))
                return;

            var tenantName = lease.Tenant?.FullName ?? "Tenant";
            var propertyName = lease.Property?.Name ?? "Property";
            var propertyAddress = lease.Property?.Address ?? "";

            var subject = $"[Property Management] New Invoice #{invoice.InvoiceNumber} - {propertyName}";

            var body = $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: #4CAF50; color: white; padding: 20px; text-align: center; }}
        .content {{ padding: 20px; background-color: #f9f9f9; }}
        .invoice-details {{ background-color: white; padding: 15px; border-radius: 5px; margin: 15px 0; }}
        .detail-row {{ display: flex; justify-content: space-between; padding: 8px 0; border-bottom: 1px solid #eee; }}
        .amount {{ font-size: 24px; color: #4CAF50; font-weight: bold; }}
        .due-date {{ color: #ff5722; font-weight: bold; }}
        .btn {{ display: inline-block; padding: 12px 24px; background-color: #4CAF50; color: white; text-decoration: none; border-radius: 5px; margin-top: 15px; }}
        .footer {{ text-align: center; padding: 20px; color: #666; font-size: 12px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>New Invoice Created</h1>
        </div>
        <div class='content'>
            <p>Dear <strong>{tenantName}</strong>,</p>
            <p>A new invoice has been generated for your rental at <strong>{propertyName}</strong>.</p>

            <div class='invoice-details'>
                <h3>Invoice Details</h3>
                <div class='detail-row'>
                    <span>Invoice Number:</span>
                    <span><strong>{invoice.InvoiceNumber}</strong></span>
                </div>
                <div class='detail-row'>
                    <span>Property:</span>
                    <span>{propertyName}</span>
                </div>
                <div class='detail-row'>
                    <span>Address:</span>
                    <span>{propertyAddress}</span>
                </div>
                <div class='detail-row'>
                    <span>Billing Month:</span>
                    <span><strong>{invoice.BillingMonth:MMMM yyyy}</strong></span>
                </div>
                <div class='detail-row'>
                    <span>Issue Date:</span>
                    <span>{invoice.IssueDate:dd/MM/yyyy}</span>
                </div>
                <div class='detail-row'>
                    <span>Due Date:</span>
                    <span class='due-date'>{invoice.DueDate:dd/MM/yyyy}</span>
                </div>
                <div class='detail-row'>
                    <span>Amount Due:</span>
                    <span class='amount'>{invoice.TotalAmount:N0} VND</span>
                </div>
            </div>

            <p>Please ensure payment is made before the due date to avoid late fees.</p>
            <p>You can make your payment through the tenant portal.</p>

            <center>
                <a href='#' class='btn'>Pay Now</a>
            </center>
        </div>
        <div class='footer'>
            <p>This is an automated message from Property Management System.</p>
            <p>If you have any questions, please contact your landlord.</p>
        </div>
    </div>
</body>
</html>";

            await SendEmailAsync(tenantEmail, subject, body);
        }

        /// <summary>
        /// Sends invoice created notification to landlord.
        /// </summary>
        public async Task SendInvoiceCreatedToLandlordAsync(Invoice invoice, Lease lease)
        {
            var landlordEmail = lease.Property?.Landlord?.Email;
            if (string.IsNullOrEmpty(landlordEmail))
                return;

            var landlordName = lease.Property?.Landlord?.FullName ?? "Landlord";
            var tenantName = lease.Tenant?.FullName ?? "Tenant";
            var propertyName = lease.Property?.Name ?? "Property";
            var propertyAddress = lease.Property?.Address ?? "";

            var subject = $"[Property Management] Invoice #{invoice.InvoiceNumber} Created - {propertyName}";

            var body = $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: #2196F3; color: white; padding: 20px; text-align: center; }}
        .content {{ padding: 20px; background-color: #f9f9f9; }}
        .invoice-details {{ background-color: white; padding: 15px; border-radius: 5px; margin: 15px 0; }}
        .detail-row {{ display: flex; justify-content: space-between; padding: 8px 0; border-bottom: 1px solid #eee; }}
        .amount {{ font-size: 24px; color: #2196F3; font-weight: bold; }}
        .footer {{ text-align: center; padding: 20px; color: #666; font-size: 12px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>Invoice Generated</h1>
        </div>
        <div class='content'>
            <p>Dear <strong>{landlordName}</strong>,</p>
            <p>A new invoice has been automatically generated for your property.</p>

            <div class='invoice-details'>
                <h3>Invoice Details</h3>
                <div class='detail-row'>
                    <span>Invoice Number:</span>
                    <span><strong>{invoice.InvoiceNumber}</strong></span>
                </div>
                <div class='detail-row'>
                    <span>Property:</span>
                    <span>{propertyName}</span>
                </div>
                <div class='detail-row'>
                    <span>Address:</span>
                    <span>{propertyAddress}</span>
                </div>
                <div class='detail-row'>
                    <span>Tenant:</span>
                    <span>{tenantName}</span>
                </div>
                <div class='detail-row'>
                    <span>Billing Month:</span>
                    <span><strong>{invoice.BillingMonth:MMMM yyyy}</strong></span>
                </div>
                <div class='detail-row'>
                    <span>Due Date:</span>
                    <span>{invoice.DueDate:dd/MM/yyyy}</span>
                </div>
                <div class='detail-row'>
                    <span>Amount:</span>
                    <span class='amount'>{invoice.TotalAmount:N0} VND</span>
                </div>
            </div>

            <p>The tenant has been notified about this invoice.</p>
            <p>You can track payment status in your landlord dashboard.</p>
        </div>
        <div class='footer'>
            <p>This is an automated message from Property Management System.</p>
        </div>
    </div>
</body>
</html>";

            await SendEmailAsync(landlordEmail, subject, body);
        }
    }
}
