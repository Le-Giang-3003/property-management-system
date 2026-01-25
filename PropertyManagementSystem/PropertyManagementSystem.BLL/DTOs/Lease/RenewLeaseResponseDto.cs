using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyManagementSystem.BLL.DTOs.Lease
{
    public class RenewLeaseResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int? NewLeaseId { get; set; }  // ID của lease mới
        public DateTime? NewStartDate { get; set; }
        public DateTime? NewEndDate { get; set; }
    }
}
