using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Areas.User.Controllers
{
    [Area("User")]
    [Authorize]
    public class CoursesController : Controller
    {
        public IActionResult Index()
        {
            // TODO: Implement courses listing
            return View();
        }

        public IActionResult Details(int id)
        {
            // TODO: Implement course details
            return View();
        }
    }
}
