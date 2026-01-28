using System;
using System.Collections.Generic;

namespace PropertyManagementSystem.DAL.Entities;

public partial class ViewingSchedule
{
    public int Id { get; set; }

    public int PropertyId { get; set; }

    public int LandlordId { get; set; }

    public int ViewerId { get; set; }

    public DateTime ScheduledAt { get; set; }

    public string Status { get; set; } = null!;

    public DateTime? CancelledAt { get; set; }

    public string? CancelReason { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual User Landlord { get; set; } = null!;

    public virtual Property Property { get; set; } = null!;

    public virtual User Viewer { get; set; } = null!;
}
