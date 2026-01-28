using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PropertyManagementSystem.Web.ViewModels;
using System.Diagnostics;
using PropertyManagementSystem.BLL.Services.Interface;
using PropertyManagementSystem.BLL.DTOs.Schedule;
using System.Security.Claims;

namespace PropertyManagementSystem.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IPropertyViewingService _viewingService;

        public HomeController(ILogger<HomeController> logger, IPropertyViewingService viewingService)
        {
            _logger = logger;
            _viewingService = viewingService;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdClaim, out var userId))
            {
                if (User.IsInRole("Member"))
                {
                    var viewings = await _viewingService.GetViewingsByTenantAsync(userId);
                    return View(viewings);
                }
                else if (User.IsInRole("Landlord"))
                {
                    var viewings = await _viewingService.GetViewingsByLandlordAsync(userId);
                    return View(viewings);
                }
            }
            return View(new List<PropertyViewingDto>());
        }

        [AllowAnonymous]
        public IActionResult Privacy()
        {
            Console.WriteLine("Privacy page accessed.");    
            return View();
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}
