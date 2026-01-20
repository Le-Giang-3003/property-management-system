using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PropertyManagementSystem.DAL.Entities
{
    public class NotificationPreference
    {
        [Key]
        public int PreferenceId { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        public bool EmailEnabled { get; set; } = true;
        public bool PushEnabled { get; set; } = true;
        public bool SmsEnabled { get; set; } = false;

        public bool PaymentReminders { get; set; } = true;
        public bool MaintenanceUpdates { get; set; } = true;
        public bool LeaseAlerts { get; set; } = true;
        public bool ViewingReminders { get; set; } = true;
        public bool ApplicationUpdates { get; set; } = true;
        public bool ChatMessages { get; set; } = true;
        public bool SystemAnnouncements { get; set; } = true;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public User User { get; set; }
    }
}
