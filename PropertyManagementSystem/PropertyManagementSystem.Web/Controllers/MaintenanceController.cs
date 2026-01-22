using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PropertyManagementSystem.Web.Controllers
{
    [Authorize]
    public class MaintenanceController : Controller
    {
        public MaintenanceController()
        {
        }

        public IActionResult UserMaintenanceManagement()
        {
            return View("UserMaintenanceManagement");
        }
    }
}
