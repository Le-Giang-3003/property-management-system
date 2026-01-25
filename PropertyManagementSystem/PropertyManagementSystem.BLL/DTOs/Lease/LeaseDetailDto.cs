using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyManagementSystem.BLL.DTOs.Lease
{
    public class LeaseDetailDto
    {
        public int LeaseId { get; set; }
        public string LeaseNumber { get; set; }
        public int PropertyId { get; set; }
        public string PropertyName { get; set; }
        public string PropertyAddress { get; set; }
        public int TenantId { get; set; }
        public string TenantName { get; set; }
        public string TenantEmail { get; set; }
        public string TenantPhone { get; set; }
        public int? ApplicationId { get; set; }
        public string ApplicationNumber { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal MonthlyRent { get; set; }
        public decimal SecurityDeposit { get; set; }
        public int PaymentDueDay { get; set; }
        public string Terms { get; set; }
        public string SpecialConditions { get; set; }
        public string Status { get; set; }
        public DateTime? SignedDate { get; set; }
        public bool AutoRenew { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
