using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyManagementSystem.BLL.DTOs.Lease
{
    public class LeaseListDto
    {
        public int LeaseId { get; set; }
        public string LeaseNumber { get; set; }
        public string PropertyName { get; set; }
        public string TenantName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal MonthlyRent { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
