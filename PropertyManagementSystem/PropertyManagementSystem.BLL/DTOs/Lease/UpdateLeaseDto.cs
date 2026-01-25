using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyManagementSystem.BLL.DTOs.Lease
{
    public class UpdateLeaseDto
    {
        public int LeaseId { get; set; }

        [Required]
        [Display(Name = "Ngày bắt đầu")]
        public DateTime StartDate { get; set; }

        [Required]
        [Range(1, 120)]
        [Display(Name = "Thời hạn thuê (tháng)")]
        public int LeaseDurationMonths { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal MonthlyRent { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal SecurityDeposit { get; set; }

        public string? Terms { get; set; }

        public string? SpecialConditions { get; set; }

        public bool AutoRenew { get; set; }
    }
}
