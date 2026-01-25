using System.ComponentModel.DataAnnotations;

namespace PropertyManagementSystem.Web.ViewModels.Payment
{
    public class RaiseDisputeViewModel
    {
        [Required]
        public int InvoiceId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn lý do khiếu nại")]
        public string Reason { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng mô tả chi tiết vấn đề")]
        [StringLength(500)]
        public string Description { get; set; } = null!;
    }

}
