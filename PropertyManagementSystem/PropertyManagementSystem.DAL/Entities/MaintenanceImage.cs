using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PropertyManagementSystem.DAL.Entities
{
    public class MaintenanceImage
    {
        [Key]
        public int ImageId { get; set; }

        [ForeignKey("MaintenanceRequest")]
        public int RequestId { get; set; }

        [Required, MaxLength(500)]
        public string ImageUrl { get; set; }

        [MaxLength(500)]
        public string ImagePath { get; set; }

        [MaxLength(200)]
        public string Caption { get; set; }

        [MaxLength(20)]
        public string ImageType { get; set; } = "Before"; // Before, During, After

        [ForeignKey("UploadedByUser")]
        public int UploadedBy { get; set; }

        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public MaintenanceRequest MaintenanceRequest { get; set; }
        public User UploadedByUser { get; set; }
    }
<<<<<<< HEAD
=======

>>>>>>> 7864dd8da4821481c77672150503091864b776b9
}
