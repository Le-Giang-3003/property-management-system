using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyManagementSystem.BLL.DTOs.Application
{
    public class RentalApplicationDetailDto
    {
        public int ApplicationId { get; set; }
        public string ApplicationNumber { get; set; }
        public int PropertyId { get; set; }
        public string PropertyName { get; set; }
        public int ApplicantId { get; set; }
        public string ApplicantName { get; set; }
        public string EmploymentStatus { get; set; }
        public string Employer { get; set; }
        public decimal? MonthlyIncome { get; set; }
        public string PreviousAddress { get; set; }
        public string PreviousLandlord { get; set; }
        public string PreviousLandlordPhone { get; set; }
        public int NumberOfOccupants { get; set; }
        public bool HasPets { get; set; }
        public string PetDetails { get; set; }
        public DateTime DesiredMoveInDate { get; set; }
        public string AdditionalNotes { get; set; }
        public string Status { get; set; }
        public string RejectionReason { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ReviewedAt { get; set; }
        public string ReviewedByName { get; set; }
    }
}
