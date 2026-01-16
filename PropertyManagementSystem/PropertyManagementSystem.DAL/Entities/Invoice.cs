using System;
using System.Collections.Generic;

namespace PropertyManagementSystem.DAL.Entities;

public partial class Invoice
{
    public int Id { get; set; }

    public int PaymentId { get; set; }

    public string? InvoiceNumber { get; set; }

    public int GeneratedById { get; set; }

    public DateTime GeneratedAt { get; set; }

    public int? FileId { get; set; }

    public decimal TotalAmount { get; set; }

    public virtual File? File { get; set; }

    public virtual User GeneratedBy { get; set; } = null!;

    public virtual Payment Payment { get; set; } = null!;
}
