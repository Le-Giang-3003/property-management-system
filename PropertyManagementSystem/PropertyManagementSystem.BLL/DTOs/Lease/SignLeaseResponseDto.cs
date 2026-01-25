using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyManagementSystem.BLL.DTOs.Lease
{
    public class SignLeaseResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = null!;
        public bool IsFullySigned { get; set; }
        public string LeaseStatus { get; set; } = null!;
        public DateTime? SignedDate { get; set; }
        public LeaseSignatureDto? NewSignature { get; set; }
    }
}
