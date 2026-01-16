using System;
using System.Collections.Generic;

namespace PropertyManagementSystem.DAL.Entities;

public partial class ChatMessage
{
    public int Id { get; set; }

    public int ChatRoomId { get; set; }

    public int SenderId { get; set; }

    public string Content { get; set; } = null!;

    public DateTime SentAt { get; set; }

    public bool IsRead { get; set; }

    public DateTime? ReadAt { get; set; }

    public virtual ChatRoom ChatRoom { get; set; } = null!;

    public virtual User Sender { get; set; } = null!;
}
