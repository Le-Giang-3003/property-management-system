using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyManagementSystem.BLL.DTOs.Lease
{
    public class SignLeaseDto
    {
        [Required(ErrorMessage = "Lease ID is required")]
        public int LeaseId { get; set; }

        [Required(ErrorMessage = "User ID is required")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Signer role is required")]
        [RegularExpression("^(Landlord|Tenant)$", ErrorMessage = "Role must be Landlord or Tenant")]
        public string SignerRole { get; set; } = null!;

        /// <summary>Base64 signature string or file path.</summary>
        public string? SignatureData { get; set; }

        /// <summary>Signer IP address (from Request).</summary>
        public string? IpAddress { get; set; }
    }
}
