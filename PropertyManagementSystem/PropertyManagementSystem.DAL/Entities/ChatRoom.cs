using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

<<<<<<< HEAD
namespace PropertyManagementSystem.DAL.Entities
{
    public class ChatRoom
    {
        [Key]
        public int ChatRoomId { get; set; }

        [MaxLength(200)]
        public string Name { get; set; }

        [Required, MaxLength(20)]
        public string Type { get; set; } // Property, Contract, Direct

        [ForeignKey("Property")]
        public int? PropertyId { get; set; }

        [ForeignKey("Lease")]
        public int? LeaseId { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastMessageAt { get; set; }

        // Navigation
        public Property Property { get; set; }
        public Lease Lease { get; set; }
        public ICollection<ChatParticipant> Participants { get; set; }
        public ICollection<ChatMessage> Messages { get; set; }
    }
=======
namespace PropertyManagementSystem.DAL.Entities;

public class ChatRoom
{
    [Key]
    public int ChatRoomId { get; set; }

    [MaxLength(200)]
    public string Name { get; set; }

    [Required, MaxLength(20)]
    public string Type { get; set; } // Property, Contract, Direct

    [ForeignKey("Property")]
    public int? PropertyId { get; set; }

    [ForeignKey("Lease")]
    public int? LeaseId { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastMessageAt { get; set; }

    // Navigation
    public Property Property { get; set; }
    public Lease Lease { get; set; }
    public ICollection<ChatParticipant> Participants { get; set; }
    public ICollection<ChatMessage> Messages { get; set; }
>>>>>>> 7864dd8da4821481c77672150503091864b776b9
}

