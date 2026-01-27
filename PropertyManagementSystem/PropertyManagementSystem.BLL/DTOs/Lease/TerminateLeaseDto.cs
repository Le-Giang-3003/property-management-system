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
        [Required(ErrorMessage = "Termination reason is required")]
        [MinLength(10, ErrorMessage = "Reason must be at least 10 characters")]
        public string Reason { get; set; } = string.Empty;

        [Required(ErrorMessage = "Termination date is required")]
        [DataType(DataType.Date)]
        public DateTime TerminationDate { get; set; }
    }
}
