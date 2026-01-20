using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

<<<<<<< HEAD
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
=======
namespace PropertyManagementSystem.DAL.Entities;

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
>>>>>>> 7864dd8da4821481c77672150503091864b776b9
}
