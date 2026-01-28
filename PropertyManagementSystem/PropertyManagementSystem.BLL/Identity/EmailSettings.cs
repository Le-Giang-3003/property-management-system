namespace PropertyManagementSystem.BLL.Identity
{
    /// <summary>
    /// Email settings for SMTP configuration.
    /// </summary>
    public class EmailSettings
    {
        /// <summary>
        /// Gets or sets the SMTP server.
        /// </summary>
        /// <value>
        /// The SMTP server.
        /// </value>
        public string SmtpServer { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the SMTP port.
        /// </summary>
        /// <value>
        /// The SMTP port.
        /// </value>
        public int SmtpPort { get; set; }
        /// <summary>
        /// Gets or sets the name of the sender.
        /// </summary>
        /// <value>
        /// The name of the sender.
        /// </value>
        public string SenderName { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the sender email.
        /// </summary>
        /// <value>
        /// The sender email.
        /// </value>
        public string SenderEmail { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>
        /// The password.
        /// </value>
        public string Password { get; set; } = string.Empty;
    }
}
