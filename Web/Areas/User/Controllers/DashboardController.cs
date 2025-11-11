using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Web.Areas.User.Controllers
{
    [Area("User")]
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly IUserRepository _userRepository;

        public DashboardController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<IActionResult> Index()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var user = await _userRepository.GetUserByIdAsync(userId);

            if (user == null)
            {
                return RedirectToAction("Logout", "Account", new { area = "" });
            }

            ViewBag.UserName = $"{user.FirstName} {user.LastName}".Trim();
            ViewBag.Email = user.Email;
            ViewBag.RegisterDate = user.RegisterDate.ToString("MMMM dd, yyyy");

            return View();
        }
    }
}
