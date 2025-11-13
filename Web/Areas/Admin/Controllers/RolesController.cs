using DataLayer.Context;
using DataLayer.Entities.Permission;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class RolesController : Controller
    {
        private readonly WebContext _context;
        public RolesController(WebContext context)
        {
            _context = context;
        }

        [Authorize(Policy = "Admin.Roles.View")]
        public async Task<IActionResult> Index()
        {
            ViewBag.Roles = await _context.Roles.ToListAsync();
            return View();
        }

        [Authorize(Policy = "Admin.Roles.Create")]
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Admin.Roles.Create")]
        public async Task<IActionResult> Create(Role role)
        {
            if (!ModelState.IsValid) return View(role);
            role.CreateDate = DateTime.Now; role.IsActive = true;
            _context.Roles.Add(role);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Policy = "Admin.Roles.AssignPermissions")]
        public async Task<IActionResult> ManagePermissions(int id)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role == null) return NotFound();
            var permissions = await _context.Permissions.ToListAsync();
            var rolePermIds = await _context.RolePermissions.Where(rp => rp.RoleId == id).Select(rp => rp.PermissionId).ToListAsync();
            ViewBag.Role = role; ViewBag.Permissions = permissions; ViewBag.RolePermissionIds = rolePermIds;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Admin.Roles.AssignPermissions")]
        public async Task<IActionResult> ManagePermissions(int id, int[] selectedPermissions)
        {
            var current = _context.RolePermissions.Where(rp => rp.RoleId == id);
            _context.RolePermissions.RemoveRange(current);
            foreach (var pid in selectedPermissions.Distinct())
            {
                _context.RolePermissions.Add(new RolePermission { RoleId = id, PermissionId = pid });
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Policy = "Admin.Users.AssignRoles")]
        public async Task<IActionResult> AssignUserRole(int userId)
        {
            ViewBag.UserId = userId;
            ViewBag.Roles = await _context.Roles.Where(r => r.IsActive).ToListAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Admin.Users.AssignRoles")]
        public async Task<IActionResult> AssignUserRole(int userId, int roleId)
        {
            var exists = await _context.UserRoles.AnyAsync(ur => ur.UserId == userId && ur.RoleId == roleId);
            if (!exists)
            {
                _context.UserRoles.Add(new UserRole { UserId = userId, RoleId = roleId, AssignedDate = DateTime.Now });
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index", "Users");
        }
    }
}