using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyManagementSystem.BLL.DTOs.Application
{
    public class RentalApplicationListDto
    {
        public int ApplicationId { get; set; }
        public string ApplicationNumber { get; set; }
        public int PropertyId { get; set; }
        public string PropertyName { get; set; }
        public string ApplicantName { get; set; }
        public string Status { get; set; }
        public DateTime DesiredMoveInDate { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
