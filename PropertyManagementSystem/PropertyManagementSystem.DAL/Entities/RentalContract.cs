using System;
using System.Collections.Generic;

namespace PropertyManagementSystem.DAL.Entities;

public partial class RentalContract
{
    public int Id { get; set; }

    public int PropertyId { get; set; }

    public int LandlordId { get; set; }

    public int TenantId { get; set; }

    public string? ContractNumber { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public decimal RentAmount { get; set; }

    public decimal? DepositAmount { get; set; }

    public string PaymentCycle { get; set; } = null!;

    public string Status { get; set; } = null!;

    public DateTime? SignedAtLandlord { get; set; }

    public DateTime? SignedAtTenant { get; set; }

    public DateTime? TerminatedAt { get; set; }

    public string? TerminationReason { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<ChatRoom> ChatRooms { get; set; } = new List<ChatRoom>();

    public virtual ICollection<ContractFile> ContractFiles { get; set; } = new List<ContractFile>();

    public virtual User Landlord { get; set; } = null!;

    public virtual ICollection<MaintenanceRequest> MaintenanceRequests { get; set; } = new List<MaintenanceRequest>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual Property Property { get; set; } = null!;

    public virtual User Tenant { get; set; } = null!;
}
