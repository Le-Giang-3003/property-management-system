using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyManagementSystem.BLL.DTOs.Lease
{
    public class RenewLeaseDto
    {
        public int LeaseId { get; set; }
        public int ExtensionMonths { get; set; }  // Số tháng gia hạn
        public decimal? NewMonthlyRent { get; set; }  // Tiền thuê mới (null = giữ nguyên)
        public decimal? NewSecurityDeposit { get; set; }  // Đặt cọc mới (null = giữ nguyên)
        public string? AdditionalTerms { get; set; }  // Điều khoản bổ sung
        public bool AutoRenew { get; set; }  // Có tự động gia hạn không
    }

    
}
