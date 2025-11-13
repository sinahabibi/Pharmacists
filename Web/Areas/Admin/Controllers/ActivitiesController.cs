using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DataLayer.Context;
using Microsoft.EntityFrameworkCore;

namespace Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class ActivitiesController : Controller
    {
        private readonly WebContext _context;

        public ActivitiesController(WebContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var activities = await _context.RecentActivities
                .Include(a => a.RecentActivityPriority)
                .OrderByDescending(a => a.CreateDate)
                .Take(100)
                .ToListAsync();
            return View(activities);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var activity = await _context.RecentActivities.FirstOrDefaultAsync(a => a.Id == id);
            if (activity == null)
            {
                return Json(new { success = false, message = "فعالیت یافت نشد" });
            }

            _context.RecentActivities.Remove(activity);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "فعالیت با موفقیت حذف شد" });
        }

        [HttpPost]
        public async Task<IActionResult> ClearAll()
        {
            var activities = await _context.RecentActivities.ToListAsync();
            _context.RecentActivities.RemoveRange(activities);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "تمام فعالیت‌ها پاک شدند" });
        }
    }
}
