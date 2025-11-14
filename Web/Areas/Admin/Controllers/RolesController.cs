using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DataLayer.Context;
using Microsoft.EntityFrameworkCore;
using Web.Attributes;
using System.Security.Claims;

namespace Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class RolesController : Controller
    {
        private readonly WebContext _context;

        public RolesController(WebContext context)
        {
            _context = context;
        }

        [Permission("Roles.View")]
        public async Task<IActionResult> Index()
        {
            var roles = await _context.Roles
                .Include(r => r.RolePermissions)
                    .ThenInclude(rp => rp.Permission)
                .Include(r => r.UserRoles)
                .OrderByDescending(r => r.CreateDate)
                .ToListAsync();

            return View(roles);
        }

        [Permission("Roles.View")]
        public async Task<IActionResult> Details(int id)
        {
            var role = await _context.Roles
                .Include(r => r.RolePermissions)
                    .ThenInclude(rp => rp.Permission)
                .Include(r => r.UserRoles)
                    .ThenInclude(ur => ur.User)
                .FirstOrDefaultAsync(r => r.RoleId == id);

            if (role == null)
            {
                TempData["ErrorMessage"] = "Role not found";
                return RedirectToAction(nameof(Index));
            }

            return View(role);
        }

        [Permission("Roles.Create")]
        public IActionResult Create()
        {
            ViewBag.Permissions = _context.Permissions.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Permission("Roles.Create")]
        public async Task<IActionResult> Create(DataLayer.Entities.Permission.Role role, List<int> SelectedPermissions)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Permissions = _context.Permissions.ToList();
                return View(role);
            }

            // Check if role name already exists
            var existingRole = await _context.Roles
                .FirstOrDefaultAsync(r => r.RoleName == role.RoleName);
            
            if (existingRole != null)
            {
                ModelState.AddModelError("RoleName", "A role with this name already exists");
                ViewBag.Permissions = _context.Permissions.ToList();
                return View(role);
            }

            // Set creation date
            role.CreateDate = DateTime.Now;

            // Add role to database
            _context.Roles.Add(role);
            await _context.SaveChangesAsync();

            // Add selected permissions
            if (SelectedPermissions != null && SelectedPermissions.Any())
            {
                foreach (var permissionId in SelectedPermissions)
                {
                    _context.RolePermissions.Add(new DataLayer.Entities.Permission.RolePermission
                    {
                        RoleId = role.RoleId,
                        PermissionId = permissionId
                    });
                }
                await _context.SaveChangesAsync();
            }

            TempData["SuccessMessage"] = $"Role '{role.DisplayName}' created successfully";
            return RedirectToAction(nameof(Index));
        }

        [Permission("Roles.Edit")]
        public async Task<IActionResult> Edit(int id)
        {
            var role = await _context.Roles
                .Include(r => r.RolePermissions)
                .FirstOrDefaultAsync(r => r.RoleId == id);

            if (role == null)
            {
                TempData["ErrorMessage"] = "Role not found";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Permissions = _context.Permissions.ToList();
            ViewBag.RolePermissions = role.RolePermissions.Select(rp => rp.PermissionId).ToList();

            return View(role);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Permission("Roles.Edit")]
        public async Task<IActionResult> Edit(DataLayer.Entities.Permission.Role role, List<int> SelectedPermissions)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Permissions = _context.Permissions.ToList();
                ViewBag.RolePermissions = SelectedPermissions ?? new List<int>();
                return View(role);
            }

            var existingRole = await _context.Roles
                .Include(r => r.RolePermissions)
                .FirstOrDefaultAsync(r => r.RoleId == role.RoleId);

            if (existingRole == null)
            {
                TempData["ErrorMessage"] = "Role not found";
                return RedirectToAction(nameof(Index));
            }

            // Check if another role with same name exists
            var duplicateRole = await _context.Roles
                .FirstOrDefaultAsync(r => r.RoleName == role.RoleName && r.RoleId != role.RoleId);
            
            if (duplicateRole != null)
            {
                ModelState.AddModelError("RoleName", "A role with this name already exists");
                ViewBag.Permissions = _context.Permissions.ToList();
                ViewBag.RolePermissions = SelectedPermissions ?? new List<int>();
                return View(role);
            }

            // Prevent modification of SuperAdmin role name
            if (existingRole.RoleName == "SuperAdmin" && role.RoleName != "SuperAdmin")
            {
                ModelState.AddModelError("RoleName", "Cannot change SuperAdmin role name");
                ViewBag.Permissions = _context.Permissions.ToList();
                ViewBag.RolePermissions = SelectedPermissions ?? new List<int>();
                return View(role);
            }

            // Update role properties
            existingRole.RoleName = role.RoleName;
            existingRole.DisplayName = role.DisplayName;
            existingRole.Description = role.Description;
            existingRole.IsActive = role.IsActive;

            // Update permissions
            // Remove existing permissions
            _context.RolePermissions.RemoveRange(existingRole.RolePermissions);

            // Add new permissions
            if (SelectedPermissions != null && SelectedPermissions.Any())
            {
                foreach (var permissionId in SelectedPermissions)
                {
                    _context.RolePermissions.Add(new DataLayer.Entities.Permission.RolePermission
                    {
                        RoleId = existingRole.RoleId,
                        PermissionId = permissionId
                    });
                }
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Role '{existingRole.DisplayName}' updated successfully";
            return RedirectToAction(nameof(Details), new { id = existingRole.RoleId });
        }

        [HttpPost]
        [Permission("Roles.Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var role = await _context.Roles
                .Include(r => r.UserRoles)
                .FirstOrDefaultAsync(r => r.RoleId == id);

            if (role == null)
            {
                return Json(new { success = false, message = "Role not found" });
            }

            // Check if role is assigned to any users
            if (role.UserRoles.Any())
            {
                return Json(new { success = false, message = "Cannot delete role that is assigned to users" });
            }

            // Prevent deletion of SuperAdmin role
            if (role.RoleName == "SuperAdmin")
            {
                return Json(new { success = false, message = "Cannot delete SuperAdmin role" });
            }

            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Role deleted successfully" });
        }

        [HttpPost]
        [Permission("Roles.Edit")]
        public async Task<IActionResult> ToggleActive(int id)
        {
            var role = await _context.Roles.FirstOrDefaultAsync(r => r.RoleId == id);
            
            if (role == null)
            {
                return Json(new { success = false, message = "Role not found" });
            }

            // Prevent deactivation of SuperAdmin role
            if (role.RoleName == "SuperAdmin")
            {
                return Json(new { success = false, message = "Cannot deactivate SuperAdmin role" });
            }

            role.IsActive = !role.IsActive;
            await _context.SaveChangesAsync();

            return Json(new 
            { 
                success = true, 
                isActive = role.IsActive, 
                message = role.IsActive ? "Role activated" : "Role deactivated" 
            });
        }
    }
}
