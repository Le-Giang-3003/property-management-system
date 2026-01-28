using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PropertyManagementSystem.DAL.Entities
{
    public class Property
    {
        [Key]
        public int PropertyId { get; set; }

        [ForeignKey("Landlord")]
        public int LandlordId { get; set; }

        [Required, MaxLength(200)]
        public string Name { get; set; }

        [Required, MaxLength(500)]
        public string Address { get; set; }

        [MaxLength(100)]
        public string City { get; set; }

        [MaxLength(100)]
        public string District { get; set; }

        [MaxLength(20)]
        public string ZipCode { get; set; }

        [Column(TypeName = "decimal(10,6)")]
        public decimal? Latitude { get; set; }

        [Column(TypeName = "decimal(10,6)")]
        public decimal? Longitude { get; set; }

        [Required, MaxLength(50)]
        public string PropertyType { get; set; } // Apartment, House, Condo, Studio, Villa

        public int Bedrooms { get; set; }
        public int Bathrooms { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal SquareFeet { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal RentAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? DepositAmount { get; set; }

        [MaxLength(3000)]
        public string Description { get; set; }

        [MaxLength(1000)]
        public string Amenities { get; set; } // JSON

        [MaxLength(500)]
        public string UtilitiesIncluded { get; set; } // JSON

        public bool IsFurnished { get; set; } = false;
        public bool PetsAllowed { get; set; } = false;

        [Required, MaxLength(20)]
        public string Status { get; set; } = "Available"; // Available, PendingLease , Rented, Maintenance, Unavailable

        public DateTime? AvailableFrom { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation
        public User Landlord { get; set; }
        public ICollection<PropertyImage> Images { get; set; }
        public ICollection<Lease> Leases { get; set; }  
        public ICollection<MaintenanceRequest> MaintenanceRequests { get; set; }
        public ICollection<PropertyViewing> Viewings { get; set; }
        public ICollection<RentalApplication> RentalApplications { get; set; }
        public ICollection<FavoriteProperty> FavoritedBy { get; set; }
        public PropertyAnalytics Analytics { get; set; }
        public ICollection<AIPricePrediction> PricePredictions { get; set; }
    }
}

