using Microsoft.AspNetCore.Mvc;

namespace PropertyManagementSystem.Web.ViewComponents
{
    public class HeaderViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View("Header");
        }
    }
}
