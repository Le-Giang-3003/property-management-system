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
        [Required(ErrorMessage = "Lease ID là bắt buộc")]
        public int LeaseId { get; set; }

        [Required(ErrorMessage = "User ID là bắt buộc")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Vai trò người ký là bắt buộc")]
        [RegularExpression("^(Landlord|Tenant)$", ErrorMessage = "Vai trò phải là Landlord hoặc Tenant")]
        public string SignerRole { get; set; } = null!;

        /// <summary>
        /// Base64 string của chữ ký hoặc đường dẫn file chữ ký
        /// </summary>
        public string? SignatureData { get; set; }

        /// <summary>
        /// IP address của người ký (tự động lấy từ Request)
        /// </summary>
        public string? IpAddress { get; set; }
    }
}
