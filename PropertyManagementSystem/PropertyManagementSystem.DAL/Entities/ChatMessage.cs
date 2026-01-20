using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PropertyManagementSystem.DAL.Entities
{
    public class ChatMessage
    {
        [Key]
        public int MessageId { get; set; }

        [ForeignKey("ChatRoom")]
        public int ChatRoomId { get; set; }

        [ForeignKey("Sender")]
        public int SenderId { get; set; }

        [Required, MaxLength(4000)]
        public string Content { get; set; }

        [MaxLength(20)]
        public string MessageType { get; set; } = "Text"; // Text, Image, File

        [MaxLength(500)]
        public string AttachmentUrl { get; set; }

        public bool IsEdited { get; set; } = false;
        public bool IsDeleted { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation
        public ChatRoom ChatRoom { get; set; }
        public User Sender { get; set; }
    }
}

