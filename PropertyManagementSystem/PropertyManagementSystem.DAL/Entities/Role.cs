using System.ComponentModel.DataAnnotations;

namespace PropertyManagementSystem.DAL.Entities;

public class Role
{
    [Key]
    public int RoleId { get; set; }

    [Required, MaxLength(50)]
    public string RoleName { get; set; } // Admin, Landlord, Tenant, Technician

    [MaxLength(200)]
    public string Description { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public ICollection<UserRole> UserRoles { get; set; }
}

