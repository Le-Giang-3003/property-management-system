using System;
using System.Collections.Generic;

namespace PropertyManagementSystem.DAL.Entities;

public partial class Payment
{
    public int Id { get; set; }

    public int ContractId { get; set; }

    public int TenantId { get; set; }

    public int LandlordId { get; set; }

    public DateOnly DueDate { get; set; }

    public DateOnly? PaidDate { get; set; }

    public decimal Amount { get; set; }

    public string Currency { get; set; } = null!;

    public string Status { get; set; } = null!;

    public string? PaymentMethod { get; set; }

    public string? ReferenceCode { get; set; }

    public string? Notes { get; set; }

    public bool IsDisputed { get; set; }

    public string? DisputeReason { get; set; }

    public decimal? RefundedAmount { get; set; }

    public DateTime? LastReminderSentAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual RentalContract Contract { get; set; } = null!;

    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    public virtual User Landlord { get; set; } = null!;

    public virtual User Tenant { get; set; } = null!;
}
