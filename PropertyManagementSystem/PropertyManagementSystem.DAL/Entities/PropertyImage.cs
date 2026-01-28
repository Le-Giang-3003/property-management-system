using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PropertyManagementSystem.DAL.Entities
{
    public class PropertyImage
    {
        [Key]
        public int ImageId { get; set; }

        [ForeignKey("Property")]
        public int PropertyId { get; set; }

        [Required, MaxLength(500)]
        public string ImageUrl { get; set; }

        [MaxLength(500)]
        public string ImagePath { get; set; }

        public bool IsThumbnail { get; set; } = false;
        public int DisplayOrder { get; set; } = 0;

        [MaxLength(200)]
        public string Caption { get; set; }

        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public Property Property { get; set; }
    }
}
