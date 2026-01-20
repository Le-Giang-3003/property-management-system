using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PropertyManagementSystem.DAL.Entities;

public class Payment
{
    [Key]
    public int PaymentId { get; set; }

    [ForeignKey("Invoice")]
    public int InvoiceId { get; set; }

    [Required, MaxLength(50)]
    public string PaymentNumber { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }

    public DateTime PaymentDate { get; set; }

    [Required, MaxLength(50)]
    public string PaymentMethod { get; set; } // Cash, BankTransfer, CreditCard, Momo, ZaloPay

    [MaxLength(100)]
    public string TransactionReference { get; set; }

    [MaxLength(200)]
    public string BankName { get; set; }

    [MaxLength(50)]
    public string AccountNumber { get; set; }

    [MaxLength(1000)]
    public string Notes { get; set; }

    [MaxLength(500)]
    public string ReceiptFileUrl { get; set; }

    [MaxLength(500)]
    public string ReceiptFilePath { get; set; }

    [Required, MaxLength(20)]
    public string Status { get; set; } = "Pending"; // Pending, Confirmed, Failed, Refunded, PartialRefund

    [ForeignKey("ProcessedByUser")]
    public int? ProcessedBy { get; set; }

    public DateTime? ConfirmedAt { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public Invoice Invoice { get; set; }
    public User ProcessedByUser { get; set; }
    public Refund Refund { get; set; }
}
