using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DataLayer.Context;
using Microsoft.EntityFrameworkCore;

namespace Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class SettingsController : AdminBaseController
    {
        private readonly WebContext _context;

        public SettingsController(WebContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var settings = await _context.Settings.ToListAsync();
            return View(settings);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var setting = await _context.Settings.FirstOrDefaultAsync(s => s.Id == id);
            if (setting == null)
            {
                TempData["ErrorMessage"] = "Setting not found";
                return RedirectToAction(nameof(Index));
            }
            return View(setting);
        }
    }
}
