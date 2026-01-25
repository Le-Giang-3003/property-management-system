using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyManagementSystem.BLL.DTOs.Lease
{
    public class CreateLeaseDto
    {
        [Required(ErrorMessage = "Vui lòng chọn đơn xin thuê")]
        public int ApplicationId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn thời hạn thuê")]
        [Range(1, 120, ErrorMessage = "Thời hạn thuê phải từ 1-120 tháng")]
        [Display(Name = "Thời hạn thuê (tháng)")]
        public int LeaseDurationMonths { get; set; } = 12;

        [Required(ErrorMessage = "Vui lòng nhập tiền thuê hàng tháng")]
        [Range(0, double.MaxValue, ErrorMessage = "Tiền thuê phải là số dương")]
        [Display(Name = "Tiền thuê hàng tháng (VNĐ)")]
        public decimal MonthlyRent { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tiền đặt cọc")]
        [Range(0, double.MaxValue, ErrorMessage = "Tiền cọc phải là số dương")]
        [Display(Name = "Tiền đặt cọc (VNĐ)")]
        public decimal SecurityDeposit { get; set; }

        [Display(Name = "Điều khoản hợp đồng")]
        public string Terms { get; set; }

        [Display(Name = "Điều kiện đặc biệt")]
        public string SpecialConditions { get; set; }

        [Display(Name = "Tự động gia hạn")]
        public bool AutoRenew { get; set; } = false;
    } 
}
