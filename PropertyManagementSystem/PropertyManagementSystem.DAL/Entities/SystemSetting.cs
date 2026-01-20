using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PropertyManagementSystem.DAL.Entities
{
    public class SystemSetting
    {
        [Key]
        public int SettingId { get; set; }

        [Required, MaxLength(100)]
        public string SettingKey { get; set; }

        [Required]
        public string SettingValue { get; set; }

        [MaxLength(50)]
        public string Category { get; set; } // Email, Payment, General, AI, Security

        [MaxLength(500)]
        public string Description { get; set; }

        [MaxLength(50)]
        public string DataType { get; set; } = "String"; // String, Int, Bool, JSON

        public bool IsPublic { get; set; } = false;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("UpdatedByUser")]
        public int? UpdatedBy { get; set; }

        // Navigation
        public User UpdatedByUser { get; set; }
    }
}
