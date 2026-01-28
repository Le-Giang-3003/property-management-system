using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyManagementSystem.BLL.DTOs.Lease
{
    public class LeaseSignatureDto
    {
        public int SignatureId { get; set; }
        public int LeaseId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = null!;
        public string SignerRole { get; set; } = null!;

        public string? SignatureData { get; set; }
        public DateTime SignedAt { get; set; }
        public string? IpAddress { get; set; }
    }
}
