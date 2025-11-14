using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DataLayer.Context;
using Microsoft.EntityFrameworkCore;
using Web.Areas.Admin.ViewModels;
using Core.Interfaces;
using Web.Attributes;

namespace Web.Areas.Admin.Controllers
{
    public class UsersController : AdminBaseController
    {
        private readonly WebContext _context;
        private readonly IPasswordHasher _passwordHasher;

        public UsersController(WebContext context, IPasswordHasher passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        [Permission("Users.View")]
        public async Task<IActionResult> Index()
        {
            var users = await _context.Users.OrderByDescending(u => u.RegisterDate).ToListAsync();
            return View(users);
        }

        [Permission("Users.View")]
        public async Task<IActionResult> Details(int id)
        {
            var user = await _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.UserId == id);
                
            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found";
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        [Permission("Users.Create")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Permission("Users.Create")]
        public async Task<IActionResult> Create(CreateUserViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            // Check if username already exists
            var existingUsername = await _context.Users
                .FirstOrDefaultAsync(u => u.UserName == viewModel.UserName);
            if (existingUsername != null)
            {
                ModelState.AddModelError("UserName", "This username is already taken");
                return View(viewModel);
            }

            // Check if email already exists
            var existingEmail = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == viewModel.Email);
            if (existingEmail != null)
            {
                ModelState.AddModelError("Email", "This email is already registered");
                return View(viewModel);
            }

            // Check if phone number already exists
            var existingPhone = await _context.Users
                .FirstOrDefaultAsync(u => u.PhoneNumber == viewModel.PhoneNumber);
            if (existingPhone != null)
            {
                ModelState.AddModelError("PhoneNumber", "This phone number is already registered");
                return View(viewModel);
            }

            // Create new user
            var user = new DataLayer.Entities.User.User
            {
                UserName = viewModel.UserName,
                FirstName = viewModel.FirstName,
                LastName = viewModel.LastName,
                Email = viewModel.Email,
                PhoneNumber = viewModel.PhoneNumber,
                Password = _passwordHasher.HashPassword(viewModel.Password),
                IsEmailActive = viewModel.IsEmailActive,
                IsPhoneNumberActive = viewModel.IsPhoneNumberActive,
                IsBan = viewModel.IsBan,
                ActiveCode = Guid.NewGuid().ToString().Replace("-", ""),
                SecurityCode = Guid.NewGuid().ToString().Replace("-", ""),
                ActivePhoneNumberCode = new Random().Next(10000, 99999).ToString(),
                RegisterDate = DateTime.Now,
                LastChange = DateTime.Now,
                IsDelete = false,
                TryCount = 0,
                LoginWithGoogle = false
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "User created successfully";
            return RedirectToAction(nameof(Index));
        }

        [Permission("Users.Edit")]
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.UserId == id);
                
            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found";
                return RedirectToAction(nameof(Index));
            }

            // Prevent editing Admin user
            if (user.UserName.Equals("Admin", StringComparison.OrdinalIgnoreCase))
            {
                TempData["ErrorMessage"] = "Access Denied: Admin user cannot be modified";
                return RedirectToAction(nameof(Index));
            }

            // Get all active roles
            var allRoles = await _context.Roles
                .Where(r => r.IsActive)
                .OrderBy(r => r.DisplayName)
                .ToListAsync();

            // Get current user's role IDs
            var userRoleIds = user.UserRoles.Select(ur => ur.RoleId).ToList();

            var viewModel = new EditUserViewModel
            {
                UserId = user.UserId,
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                IsEmailActive = user.IsEmailActive,
                IsPhoneNumberActive = user.IsPhoneNumberActive,
                IsBan = user.IsBan,
                SelectedRoleIds = userRoleIds,
                AvailableRoles = allRoles.Select(r => new RoleSelectionViewModel
                {
                    RoleId = r.RoleId,
                    RoleName = r.RoleName,
                    DisplayName = r.DisplayName,
                    Description = r.Description,
                    IsSelected = userRoleIds.Contains(r.RoleId)
                }).ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Permission("Users.Edit")]
        public async Task<IActionResult> Edit(EditUserViewModel viewModel)
        {
            var user = await _context.Users
                .Include(u => u.UserRoles)
                .FirstOrDefaultAsync(u => u.UserId == viewModel.UserId);
                
            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found";
                return RedirectToAction(nameof(Index));
            }

            // Prevent modification of Admin user
            if (user.UserName.Equals("Admin", StringComparison.OrdinalIgnoreCase))
            {
                TempData["ErrorMessage"] = "Access Denied: Admin user cannot be modified";
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                await LoadAvailableRoles(viewModel);
                return View(viewModel);
            }

            // Check if username already exists for another user
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.UserName == viewModel.UserName && u.UserId != viewModel.UserId);
            if (existingUser != null)
            {
                ModelState.AddModelError("UserName", "This username is already taken");
                await LoadAvailableRoles(viewModel);
                return View(viewModel);
            }

            // Prevent changing username to "Admin"
            if (viewModel.UserName.Equals("Admin", StringComparison.OrdinalIgnoreCase) && 
                !user.UserName.Equals("Admin", StringComparison.OrdinalIgnoreCase))
            {
                ModelState.AddModelError("UserName", "Cannot use 'Admin' as username - it is reserved");
                await LoadAvailableRoles(viewModel);
                return View(viewModel);
            }

            // Update user properties
            user.UserName = viewModel.UserName;
            user.FirstName = viewModel.FirstName;
            user.LastName = viewModel.LastName;
            user.Email = viewModel.Email;
            user.PhoneNumber = viewModel.PhoneNumber;
            user.IsEmailActive = viewModel.IsEmailActive;
            user.IsPhoneNumberActive = viewModel.IsPhoneNumberActive;
            user.IsBan = viewModel.IsBan;
            user.LastChange = DateTime.Now;

            // Update password if provided
            if (!string.IsNullOrEmpty(viewModel.NewPassword))
            {
                user.Password = _passwordHasher.HashPassword(viewModel.NewPassword);
            }

            // Update user roles
            await UpdateUserRoles(user.UserId, viewModel.SelectedRoleIds ?? new List<int>());

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "User information updated successfully";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Permission("Users.Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == id);
            if (user == null)
            {
                return Json(new { success = false, message = "User not found" });
            }

            // Prevent deletion of Admin user
            if (user.UserName.Equals("Admin", StringComparison.OrdinalIgnoreCase))
            {
                return Json(new { success = false, message = "Access Denied: Admin user cannot be deleted" });
            }

            user.IsDelete = true;
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "User deleted successfully" });
        }

        [HttpPost]
        [Permission("Users.Ban")]
        public async Task<IActionResult> ToggleBan([FromBody] ToggleBanRequest request)
        {
            if (request == null || request.Id <= 0)
            {
                return Json(new { success = false, message = "Invalid request" });
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == request.Id);
            if (user == null)
            {
                return Json(new { success = false, message = "User not found" });
            }

            // Prevent banning Admin user
            if (user.UserName.Equals("Admin", StringComparison.OrdinalIgnoreCase))
            {
                return Json(new { success = false, message = "Access Denied: Admin user cannot be banned" });
            }

            user.IsBan = !user.IsBan;
            await _context.SaveChangesAsync();

            return Json(new { success = true, isBan = user.IsBan, message = user.IsBan ? "User banned" : "User activated" });
        }

        private async Task LoadAvailableRoles(EditUserViewModel viewModel)
        {
            var allRoles = await _context.Roles
                .Where(r => r.IsActive)
                .OrderBy(r => r.DisplayName)
                .ToListAsync();

            viewModel.AvailableRoles = allRoles.Select(r => new RoleSelectionViewModel
            {
                RoleId = r.RoleId,
                RoleName = r.RoleName,
                DisplayName = r.DisplayName,
                Description = r.Description,
                IsSelected = viewModel.SelectedRoleIds?.Contains(r.RoleId) ?? false
            }).ToList();
        }

        private async Task UpdateUserRoles(int userId, List<int> selectedRoleIds)
        {
            // Check if this is the Admin user
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user != null && user.UserName.Equals("Admin", StringComparison.OrdinalIgnoreCase))
            {
                // Don't update Admin user's roles
                return;
            }

            // Get SuperAdmin role ID to prevent assignment/removal
            var superAdminRole = await _context.Roles
                .FirstOrDefaultAsync(r => r.RoleName == "SuperAdmin");

            // Get current user roles
            var currentUserRoles = await _context.UserRoles
                .Where(ur => ur.UserId == userId)
                .ToListAsync();

            // Remove roles that are no longer selected (except SuperAdmin)
            var rolesToRemove = currentUserRoles
                .Where(ur => !selectedRoleIds.Contains(ur.RoleId) && 
                            (superAdminRole == null || ur.RoleId != superAdminRole.RoleId))
                .ToList();
            _context.UserRoles.RemoveRange(rolesToRemove);

            // Add new roles (exclude SuperAdmin from manual assignment)
            var currentRoleIds = currentUserRoles.Select(ur => ur.RoleId).ToList();
            var rolesToAdd = selectedRoleIds
                .Where(roleId => !currentRoleIds.Contains(roleId) && 
                                (superAdminRole == null || roleId != superAdminRole.RoleId))
                .Select(roleId => new DataLayer.Entities.Permission.UserRole
                {
                    UserId = userId,
                    RoleId = roleId,
                    AssignedDate = DateTime.Now,
                    AssignedBy = null
                })
                .ToList();
            
            await _context.UserRoles.AddRangeAsync(rolesToAdd);
        }
    }

    // Request model for ToggleBan action
    public class ToggleBanRequest
    {
        public int Id { get; set; }
    }
}
