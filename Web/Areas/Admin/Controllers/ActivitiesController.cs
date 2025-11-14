using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DataLayer.Context;
using Microsoft.EntityFrameworkCore;
using Web.Attributes;

namespace Web.Areas.Admin.Controllers
{
    public class ActivitiesController : AdminBaseController
    {
        private readonly WebContext _context;

        public ActivitiesController(WebContext context)
        {
            _context = context;
        }

        [Permission("Dashboard.View")]
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
        [Permission("Dashboard.View")]
        public async Task<IActionResult> Delete(int id)
        {
            var activity = await _context.RecentActivities.FirstOrDefaultAsync(a => a.Id == id);
            if (activity == null)
            {
                return Json(new { success = false, message = "Activity not found" });
            }

            _context.RecentActivities.Remove(activity);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Activity deleted successfully" });
        }

        [HttpPost]
        [Permission("Dashboard.View")]
        public async Task<IActionResult> ClearAll()
        {
            var activities = await _context.RecentActivities.ToListAsync();
            _context.RecentActivities.RemoveRange(activities);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "All activities cleared" });
        }
    }
}
