using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;

namespace PropertyManagementSystem.DAL.Entities
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required, MaxLength(100)]
        public string Email { get; set; }

        [Required, MaxLength(255)]
        public string PasswordHash { get; set; }

        [Required, MaxLength(100)]
        public string FullName { get; set; }

        [MaxLength(20)]
        public string? PhoneNumber { get; set; }

        [MaxLength(500)]
        public string? Address { get; set; }

        [MaxLength(500)]
        public string? Avatar { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public bool IsActive { get; set; } = true;
        public bool EmailVerified { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }

        // Navigation
        public ICollection<UserRole> UserRoles { get; set; }
        public ICollection<Property> Properties { get; set; }
        public ICollection<Lease> TenantLeases { get; set; }
        public ICollection<MaintenanceRequest> TenantRequests { get; set; }
        public ICollection<MaintenanceRequest> TechnicianRequests { get; set; }
        public ICollection<PropertyViewing> ViewingRequests { get; set; }
        public ICollection<Notification> Notifications { get; set; }
        public ICollection<MaintenanceComment> Comments { get; set; }
        public ICollection<RentalApplication> RentalApplications { get; set; }
        public ICollection<FavoriteProperty> FavoriteProperties { get; set; }
        public ICollection<ChatParticipant> ChatParticipants { get; set; }
        public ICollection<ChatMessage> SentMessages { get; set; }
        public TenantPreference TenantPreference { get; set; }
        public NotificationPreference NotificationPreference { get; set; }
    }
}
