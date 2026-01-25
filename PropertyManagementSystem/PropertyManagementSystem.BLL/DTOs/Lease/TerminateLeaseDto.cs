using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyManagementSystem.BLL.DTOs.Lease
{
    public class TerminateLeaseDto
    {
        [Required(ErrorMessage = "Lý do hủy hợp đồng là bắt buộc")]
        [MinLength(10, ErrorMessage = "Lý do phải có ít nhất 10 ký tự")]
        public string Reason { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ngày chấm dứt là bắt buộc")]
        [DataType(DataType.Date)]
        public DateTime TerminationDate { get; set; }
    }
}
