using Core.Interfaces;
using DataLayer.Context;
using DataLayer.Entities.Course;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CoursesController : Controller
    {
        private readonly WebContext _context;
        private readonly Core.Interfaces.IAuthorizationService _authz;

        public CoursesController(WebContext context, Core.Interfaces.IAuthorizationService authz)
        {
            _context = context;
            _authz = authz;
        }

        [Authorize(Policy = "Admin.Courses.View")]
        public async Task<IActionResult> Index()
        {
            var courses = await _context.Courses
                .Select(c => new
                {
                    c.CourseId,
                    c.Title,
                    c.Price,
                    c.IsActive,
                    StudentsCount = _context.CourseEnrollments.Count(e => e.CourseId == c.CourseId)
                }).ToListAsync();

            ViewBag.Courses = courses;
            return View();
        }

        [Authorize(Policy = "Admin.Courses.Create")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Admin.Courses.Create")]
        public async Task<IActionResult> Create(Course course)
        {
            if (!ModelState.IsValid)
                return View(course);

            course.CreateDate = DateTime.Now;
            _context.Courses.Add(course);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Policy = "Admin.Courses.Edit")]
        public async Task<IActionResult> Edit(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null) return NotFound();
            return View(course);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Admin.Courses.Edit")]
        public async Task<IActionResult> Edit(int id, Course model)
        {
            if (id != model.CourseId) return BadRequest();
            if (!ModelState.IsValid) return View(model);

            var course = await _context.Courses.FindAsync(id);
            if (course == null) return NotFound();

            course.Title = model.Title;
            course.Description = model.Description;
            course.Price = model.Price;
            course.ImageName = model.ImageName;
            course.DurationHours = model.DurationHours;
            course.IsActive = model.IsActive;
            course.LastUpdate = DateTime.Now;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Policy = "Admin.Courses.Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null) return NotFound();
            return View(course);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Admin.Courses.Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null) return NotFound();
            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Policy = "Admin.Courses.View")]
        public async Task<IActionResult> Details(int id)
        {
            var course = await _context.Courses
                .Where(c => c.CourseId == id)
                .Select(c => new
                {
                    c.CourseId,
                    c.Title,
                    c.Description,
                    c.Price,
                    c.IsActive,
                    Enrollments = _context.CourseEnrollments
                        .Where(e => e.CourseId == id)
                        .Select(e => new { e.UserId, e.EnrollmentDate, e.IsPaid, e.ProgressPercentage })
                        .ToList()
                })
                .FirstOrDefaultAsync();
            if (course == null) return NotFound();
            ViewBag.Course = course;
            return View();
        }

        [Authorize(Policy = "Admin.Courses.ManageEnrollments")]
        public async Task<IActionResult> AddStudent(int id)
        {
            ViewBag.CourseId = id;
            // A simple list for selection; for a production UI, add search/autocomplete
            ViewBag.Users = await _context.Users.Select(u => new { u.UserId, u.UserName }).ToListAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Admin.Courses.ManageEnrollments")]
        public async Task<IActionResult> AddStudent(int courseId, int userId)
        {
            var exists = await _context.CourseEnrollments.AnyAsync(e => e.CourseId == courseId && e.UserId == userId);
            if (!exists)
            {
                _context.CourseEnrollments.Add(new CourseEnrollment
                {
                    CourseId = courseId,
                    UserId = userId,
                    EnrollmentDate = DateTime.Now,
                    IsCompleted = false,
                    ProgressPercentage = 0,
                    IsPaid = false
                });
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Details), new { id = courseId });
        }
    }
}