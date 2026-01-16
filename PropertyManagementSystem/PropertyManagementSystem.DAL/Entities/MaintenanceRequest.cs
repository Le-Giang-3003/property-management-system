using System;
using System.Collections.Generic;

namespace PropertyManagementSystem.DAL.Entities;

public partial class MaintenanceRequest
{
    public int Id { get; set; }

    public int PropertyId { get; set; }

    public int? ContractId { get; set; }

    public int TenantId { get; set; }

    public int LandlordId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public string Priority { get; set; } = null!;

    public string Status { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? ClosedAt { get; set; }

    public int? ClosedById { get; set; }

    public virtual User? ClosedBy { get; set; }

    public virtual RentalContract? Contract { get; set; }

    public virtual User Landlord { get; set; } = null!;

    public virtual ICollection<MaintenanceAssignment> MaintenanceAssignments { get; set; } = new List<MaintenanceAssignment>();

    public virtual Property Property { get; set; } = null!;

    public virtual User Tenant { get; set; } = null!;
}
