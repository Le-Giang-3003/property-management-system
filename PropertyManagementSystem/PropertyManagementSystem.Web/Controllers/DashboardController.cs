using Microsoft.AspNetCore.Mvc;

namespace PropertyManagementSystem.Web.Controllers
{
    /// <summary>
    /// Controller for routing to the dashboard views base on role.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.Controller" />
    public class DashboardController : Controller
    {
        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            return View();
        }
    }
}
