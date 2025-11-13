using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DataLayer.Context;
using Microsoft.EntityFrameworkCore;

namespace Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class UsersController : Controller
    {
        private readonly WebContext _context;

        public UsersController(WebContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _context.Users.OrderByDescending(u => u.RegisterDate).ToListAsync();
            return View(users);
        }

        public async Task<IActionResult> Details(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == id);
            if (user == null)
            {
                TempData["ErrorMessage"] = "کاربر یافت نشد";
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == id);
            if (user == null)
            {
                return Json(new { success = false, message = "کاربر یافت نشد" });
            }

            user.IsDelete = true;
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "کاربر با موفقیت حذف شد" });
        }

        [HttpPost]
        public async Task<IActionResult> ToggleBan(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == id);
            if (user == null)
            {
                return Json(new { success = false, message = "کاربر یافت نشد" });
            }

            user.IsBan = !user.IsBan;
            await _context.SaveChangesAsync();

            return Json(new { success = true, isBan = user.IsBan, message = user.IsBan ? "کاربر مسدود شد" : "کاربر فعال شد" });
        }
    }
}
