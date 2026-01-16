using System;
using System.Collections.Generic;

namespace PropertyManagementSystem.DAL.Entities;

public partial class ContractFile
{
    public int Id { get; set; }

    public int ContractId { get; set; }

    public int FileId { get; set; }

    public string? FileType { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual RentalContract Contract { get; set; } = null!;

    public virtual File File { get; set; } = null!;
}
