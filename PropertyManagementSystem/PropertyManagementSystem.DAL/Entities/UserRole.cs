using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PropertyManagementSystem.DAL.Entities;

public class UserRole
{
    [Key]
    public int UserRoleId { get; set; }

    [ForeignKey("User")]
    public int UserId { get; set; }

    [ForeignKey("Role")]
    public int RoleId { get; set; }

    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

    [ForeignKey("AssignedByUser")]
    public int? AssignedBy { get; set; }

    // Navigation
    public User User { get; set; }
    public Role Role { get; set; }
    public User AssignedByUser { get; set; }
}

