using System;
using System.Collections.Generic;

namespace PropertyManagementSystem.DAL.Entities;

public partial class PropertyImage
{
    public int Id { get; set; }

    public int PropertyId { get; set; }

    public int? FileId { get; set; }

    public string? ImageUrl { get; set; }

    public bool IsPrimary { get; set; }

    public int? SortOrder { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual File? File { get; set; }

    public virtual Property Property { get; set; } = null!;
}
