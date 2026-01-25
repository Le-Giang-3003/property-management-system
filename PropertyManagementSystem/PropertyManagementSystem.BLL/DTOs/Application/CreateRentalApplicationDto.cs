using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyManagementSystem.BLL.DTOs.Application
{
    public class CreateRentalApplicationDto
    {
        public int PropertyId { get; set; }
        public string EmploymentStatus { get; set; }
        public string Employer { get; set; }
        public decimal? MonthlyIncome { get; set; }
        public string PreviousAddress { get; set; }
        public string PreviousLandlord { get; set; }
        public string PreviousLandlordPhone { get; set; }
        public int NumberOfOccupants { get; set; } = 1;
        public bool HasPets { get; set; }
        public string PetDetails { get; set; }
        public DateTime DesiredMoveInDate { get; set; }
        public string AdditionalNotes { get; set; }
    }
}
