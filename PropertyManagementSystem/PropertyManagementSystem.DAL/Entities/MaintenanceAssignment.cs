using System;
using System.Collections.Generic;

namespace PropertyManagementSystem.DAL.Entities;

public partial class MaintenanceAssignment
{
    public int Id { get; set; }

    public int RequestId { get; set; }

    public int TechnicianId { get; set; }

    public int AssignedById { get; set; }

    public DateTime AssignedAt { get; set; }

    public string Status { get; set; } = null!;

    public string? Notes { get; set; }

    public virtual User AssignedBy { get; set; } = null!;

    public virtual MaintenanceRequest Request { get; set; } = null!;

    public virtual User Technician { get; set; } = null!;
}
