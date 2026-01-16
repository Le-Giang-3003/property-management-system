using System;
using System.Collections.Generic;

namespace PropertyManagementSystem.DAL.Entities;

public partial class RentalApplication
{
    public int Id { get; set; }

    public int PropertyId { get; set; }

    public int ApplicantId { get; set; }

    public string Status { get; set; } = null!;

    public DateTime AppliedAt { get; set; }

    public DateTime? ApprovedAt { get; set; }

    public DateTime? RejectedAt { get; set; }

    public string? RejectedReason { get; set; }

    public string? Note { get; set; }

    public virtual User Applicant { get; set; } = null!;

    public virtual Property Property { get; set; } = null!;
}
