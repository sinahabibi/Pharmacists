using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DataLayer.Context;
using Microsoft.EntityFrameworkCore;
using Web.Areas.Admin.ViewModels;
using Core.Interfaces;
using Web.Attributes;

namespace Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class UsersController : Controller
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
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == id);
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
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == id);
            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found";
                return RedirectToAction(nameof(Index));
            }

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
                IsBan = user.IsBan
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Permission("Users.Edit")]
        public async Task<IActionResult> Edit(EditUserViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == viewModel.UserId);
            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found";
                return RedirectToAction(nameof(Index));
            }

            // Check if username already exists for another user
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.UserName == viewModel.UserName && u.UserId != viewModel.UserId);
            if (existingUser != null)
            {
                ModelState.AddModelError("UserName", "This username is already taken");
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

            user.IsBan = !user.IsBan;
            await _context.SaveChangesAsync();

            return Json(new { success = true, isBan = user.IsBan, message = user.IsBan ? "User banned" : "User activated" });
        }
    }

    // Request model for ToggleBan action
    public class ToggleBanRequest
    {
        public int Id { get; set; }
    }
}
