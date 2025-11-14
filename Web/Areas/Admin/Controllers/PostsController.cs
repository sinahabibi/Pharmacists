using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DataLayer.Context;
using Microsoft.EntityFrameworkCore;
using Web.Attributes;

namespace Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class PostsController : Controller
    {
        private readonly WebContext _context;

        public PostsController(WebContext context)
        {
            _context = context;
        }

        [Permission("Dashboard.View")]
        public async Task<IActionResult> Index()
        {
            var posts = await _context.Posts
                .Include(p => p.User)
                .OrderByDescending(p => p.CreateDate)
                .ToListAsync();
            return View(posts);
        }

        [Permission("Dashboard.View")]
        public IActionResult Create()
        {
            return View();
        }

        [Permission("Dashboard.View")]
        public async Task<IActionResult> Edit(int id)
        {
            var post = await _context.Posts.FirstOrDefaultAsync(p => p.PostId == id);
            if (post == null)
            {
                TempData["ErrorMessage"] = "Post not found";
                return RedirectToAction(nameof(Index));
            }
            return View(post);
        }

        [HttpPost]
        [Permission("Dashboard.View")]
        public async Task<IActionResult> Delete(int id)
        {
            var post = await _context.Posts.FirstOrDefaultAsync(p => p.PostId == id);
            if (post == null)
            {
                return Json(new { success = false, message = "Post not found" });
            }

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Post deleted successfully" });
        }

        [HttpPost]
        [Permission("Dashboard.View")]
        public async Task<IActionResult> TogglePublish(int id)
        {
            var post = await _context.Posts.FirstOrDefaultAsync(p => p.PostId == id);
            if (post == null)
            {
                return Json(new { success = false, message = "Post not found" });
            }

            post.IsPublish = !post.IsPublish;
            await _context.SaveChangesAsync();

            return Json(new { success = true, isPublish = post.IsPublish, message = post.IsPublish ? "Post published" : "Post unpublished" });
        }
    }
}
