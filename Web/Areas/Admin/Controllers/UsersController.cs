using DataLayer.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UsersController : Controller
    {
        private readonly WebContext _context;
        public UsersController(WebContext context)
        {
            _context = context;
        }

        [Authorize(Policy = "Admin.Users.View")]
        public async Task<IActionResult> Index()
        {
            var users = await _context.Users.OrderByDescending(u => u.RegisterDate)
                .Take(200)
                .Select(u => new { u.UserId, u.UserName, u.Email, u.IsBan, u.RegisterDate })
                .ToListAsync();
            ViewBag.Users = users;
            return View();
        }

        [Authorize(Policy = "Admin.Users.Ban")]
        public async Task<IActionResult> ToggleBan(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();
            user.IsBan = !user.IsBan;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}