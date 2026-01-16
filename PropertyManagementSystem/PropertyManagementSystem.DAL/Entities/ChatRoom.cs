using System;
using System.Collections.Generic;

namespace PropertyManagementSystem.DAL.Entities;

public partial class ChatRoom
{
    public int Id { get; set; }

    public int? PropertyId { get; set; }

    public int? ContractId { get; set; }

    public int CreatedById { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<ChatMessage> ChatMessages { get; set; } = new List<ChatMessage>();

    public virtual RentalContract? Contract { get; set; }

    public virtual User CreatedBy { get; set; } = null!;

    public virtual Property? Property { get; set; }
}
