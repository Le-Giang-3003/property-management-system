using System;
using System.Collections.Generic;

namespace PropertyManagementSystem.DAL.Entities;

public partial class Property
{
    public int Id { get; set; }

    public int LandlordId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public string Address { get; set; } = null!;

    public string City { get; set; } = null!;

    public string? District { get; set; }

    public decimal? Latitude { get; set; }

    public decimal? Longitude { get; set; }

    public string PropertyType { get; set; } = null!;

    public int? Bedrooms { get; set; }

    public int? Bathrooms { get; set; }

    public decimal? Area { get; set; }

    public decimal? BaseRentPrice { get; set; }

    public string Status { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<ChatRoom> ChatRooms { get; set; } = new List<ChatRoom>();

    public virtual User Landlord { get; set; } = null!;

    public virtual ICollection<MaintenanceRequest> MaintenanceRequests { get; set; } = new List<MaintenanceRequest>();

    public virtual ICollection<PropertyImage> PropertyImages { get; set; } = new List<PropertyImage>();

    public virtual ICollection<RentalApplication> RentalApplications { get; set; } = new List<RentalApplication>();

    public virtual ICollection<RentalContract> RentalContracts { get; set; } = new List<RentalContract>();

    public virtual ICollection<ViewingSchedule> ViewingSchedules { get; set; } = new List<ViewingSchedule>();
}
