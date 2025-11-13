using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "Admin.Dashboard.View")]
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}