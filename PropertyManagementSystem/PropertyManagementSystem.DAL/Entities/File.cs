using System;
using System.Collections.Generic;

namespace PropertyManagementSystem.DAL.Entities;

public partial class File
{
    public int Id { get; set; }

    public string FileName { get; set; } = null!;

    public string FilePath { get; set; } = null!;

    public string? ContentType { get; set; }

    public long? SizeBytes { get; set; }

    public int UploadedById { get; set; }

    public DateTime UploadedAt { get; set; }

    public string? EntityType { get; set; }

    public int? EntityId { get; set; }

    public virtual ICollection<ContractFile> ContractFiles { get; set; } = new List<ContractFile>();

    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    public virtual ICollection<PropertyImage> PropertyImages { get; set; } = new List<PropertyImage>();

    public virtual User UploadedBy { get; set; } = null!;
}
