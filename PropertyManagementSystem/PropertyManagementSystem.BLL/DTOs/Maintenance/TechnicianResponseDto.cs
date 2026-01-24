using System.ComponentModel.DataAnnotations;

namespace PropertyManagementSystem.BLL.DTOs.Maintenance
{
    public class TechnicianResponseDto : IValidatableObject
    {
        [Required(ErrorMessage = "Request ID is required")]
        public int RequestId { get; set; }

        [Required(ErrorMessage = "Status is required")]
        public string Status { get; set; } // Accepted or Rejected

        public decimal? EstimatedCost { get; set; }

        [MaxLength(2000, ErrorMessage = "Notes cannot exceed 2000 characters")]
        public string Notes { get; set; }

        [MaxLength(3000, ErrorMessage = "Rejection reason cannot exceed 3000 characters")]
        public string RejectionReason { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.Equals(Status, "Accepted", StringComparison.OrdinalIgnoreCase))
            {
                if (!EstimatedCost.HasValue || EstimatedCost.Value <= 0)
                {
                    yield return new ValidationResult(
                        "Estimated cost is required and must be greater than 0 when accepting assignment",
                        new[] { nameof(EstimatedCost) });
                }
            }
            else if (string.Equals(Status, "Rejected", StringComparison.OrdinalIgnoreCase))
            {
                if (string.IsNullOrWhiteSpace(RejectionReason))
                {
                    yield return new ValidationResult(
                        "Rejection reason is required when rejecting assignment",
                        new[] { nameof(RejectionReason) });
                }
            }
        }
    }
}
