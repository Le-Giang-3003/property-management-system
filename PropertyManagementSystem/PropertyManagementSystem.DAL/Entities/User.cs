using System;
using System.Collections.Generic;

namespace PropertyManagementSystem.DAL.Entities;

public partial class User
{
    public int Id { get; set; }

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public string? PhoneNumber { get; set; }

    public string? AvatarUrl { get; set; }

    public string? Address { get; set; }

    public DateOnly? DateOfBirth { get; set; }

    public string Status { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<ChatMessage> ChatMessages { get; set; } = new List<ChatMessage>();

    public virtual ICollection<ChatRoom> ChatRooms { get; set; } = new List<ChatRoom>();

    public virtual ICollection<File> Files { get; set; } = new List<File>();

    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    public virtual ICollection<MaintenanceAssignment> MaintenanceAssignmentAssignedBies { get; set; } = new List<MaintenanceAssignment>();

    public virtual ICollection<MaintenanceAssignment> MaintenanceAssignmentTechnicians { get; set; } = new List<MaintenanceAssignment>();

    public virtual ICollection<MaintenanceRequest> MaintenanceRequestClosedBies { get; set; } = new List<MaintenanceRequest>();

    public virtual ICollection<MaintenanceRequest> MaintenanceRequestLandlords { get; set; } = new List<MaintenanceRequest>();

    public virtual ICollection<MaintenanceRequest> MaintenanceRequestTenants { get; set; } = new List<MaintenanceRequest>();

    public virtual ICollection<Payment> PaymentLandlords { get; set; } = new List<Payment>();

    public virtual ICollection<Payment> PaymentTenants { get; set; } = new List<Payment>();

    public virtual ICollection<Property> Properties { get; set; } = new List<Property>();

    public virtual ICollection<RentalApplication> RentalApplications { get; set; } = new List<RentalApplication>();

    public virtual ICollection<RentalContract> RentalContractLandlords { get; set; } = new List<RentalContract>();

    public virtual ICollection<RentalContract> RentalContractTenants { get; set; } = new List<RentalContract>();

    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

    public virtual ICollection<ViewingSchedule> ViewingScheduleLandlords { get; set; } = new List<ViewingSchedule>();

    public virtual ICollection<ViewingSchedule> ViewingScheduleViewers { get; set; } = new List<ViewingSchedule>();
}
