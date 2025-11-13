using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DataLayer.Context;
using Microsoft.EntityFrameworkCore;

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

        public async Task<IActionResult> Index()
        {
            var posts = await _context.Posts
                .Include(p => p.User)
                .OrderByDescending(p => p.CreateDate)
                .ToListAsync();
            return View(posts);
        }

        public IActionResult Create()
        {
            return View();
        }

        public async Task<IActionResult> Edit(int id)
        {
            var post = await _context.Posts.FirstOrDefaultAsync(p => p.PostId == id);
            if (post == null)
            {
                TempData["ErrorMessage"] = "پست یافت نشد";
                return RedirectToAction(nameof(Index));
            }
            return View(post);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var post = await _context.Posts.FirstOrDefaultAsync(p => p.PostId == id);
            if (post == null)
            {
                return Json(new { success = false, message = "پست یافت نشد" });
            }

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "پست با موفقیت حذف شد" });
        }

        [HttpPost]
        public async Task<IActionResult> TogglePublish(int id)
        {
            var post = await _context.Posts.FirstOrDefaultAsync(p => p.PostId == id);
            if (post == null)
            {
                return Json(new { success = false, message = "پست یافت نشد" });
            }

            post.IsPublish = !post.IsPublish;
            await _context.SaveChangesAsync();

            return Json(new { success = true, isPublish = post.IsPublish, message = post.IsPublish ? "پست منتشر شد" : "پست از انتشار خارج شد" });
        }
    }
}
