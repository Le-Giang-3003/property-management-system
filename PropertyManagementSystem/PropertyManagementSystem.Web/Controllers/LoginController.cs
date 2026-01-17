using Microsoft.AspNetCore.Mvc;

namespace PropertyManagementSystem.Web.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult NavigateLoginPage()
        {
            Console.Write("Qua trang Login");
            return View("Login");
        }


    }
}
