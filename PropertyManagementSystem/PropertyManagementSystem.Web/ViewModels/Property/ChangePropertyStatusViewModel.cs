namespace PropertyManagementSystem.Web.ViewModels.Property
{
    /// <summary>
    /// View model for changing the status of a property.
    /// </summary>
    public class ChangePropertyStatusViewModel
    {
        /// <summary>
        /// Gets or sets the property identifier.
        /// </summary>
        /// <value>
        /// The property identifier.
        /// </value>
        public int PropertyId { get; set; }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the current status.
        /// </summary>
        /// <value>
        /// The current status.
        /// </value>
        public string CurrentStatus { get; set; } = string.Empty;
        /// <summary>
        /// Creates new status.
        /// </summary>
        /// <value>
        /// The new status.
        /// </value>
        public string NewStatus { get; set; } = string.Empty;
    }
}
