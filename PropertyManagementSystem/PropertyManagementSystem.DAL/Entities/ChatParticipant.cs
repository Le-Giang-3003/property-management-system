using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PropertyManagementSystem.DAL.Entities
{
    public class ChatParticipant
    {
        [Key]
        public int ParticipantId { get; set; }

        [ForeignKey("ChatRoom")]
        public int ChatRoomId { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastReadAt { get; set; }

        // Navigation
        public ChatRoom ChatRoom { get; set; }
        public User User { get; set; }
    }

}
