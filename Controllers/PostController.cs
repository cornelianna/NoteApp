using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using NoteApp.Models;
using System.Security.Claims;

namespace NoteApp.Controllers
{
    public class PostController : Controller
    {
        private readonly NoteAppContext _context;

        public PostController(NoteAppContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var posts = await _context.Posts.Include(p => p.Comments).ToListAsync();
            return View(posts);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreatePost(Post post, IFormFile image)
        {
            if (image != null && image.Length > 0)
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", image.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }
                post.ImageUrl = "/uploads/" + image.FileName;
            }

            // Associate the post with the logged-in user
            post.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);  // Get the current user's ID
            post.Username = User.Identity.Name;  // Get the current user's username

            post.CreatedAt = DateTime.Now;
            post.Username = User.Identity?.Name ?? "Unknown";  // Get the current user's username
            await _context.Posts.AddAsync(post);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [Authorize]
        public async Task<IActionResult> EditPost(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null || post.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                return NotFound();  // Ensure the user can only edit their own post
            }
            return View(post);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> EditPost(int id, Post editedPost)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null || post.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                return NotFound();  // Ensure the user can only edit their own post
            }

            post.Content = editedPost.Content;
            post.ImageUrl = editedPost.ImageUrl;  // Update the image if necessary
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> DeletePost(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null || post.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                return NotFound();  // Ensure the user can only delete their own post
            }

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddComment(int postId, Comment comment)
        {
            var post = await _context.Posts.FindAsync(postId);
            if (post != null)
            {
                // Associate the comment with the logged-in user
                comment.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);  // Get the current user's ID
                comment.Username = User.Identity.Name;  // Get the current user's username

                comment.PostId = post.Id;
                comment.Username = User.Identity?.Name ?? "Unknown";  // Get the current user's username
                _context.Comments.Add(comment);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        [Authorize]
        public async Task<IActionResult> EditComment(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null || comment.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                return NotFound();  // Ensure the user can only edit their own comment
            }
            return View(comment);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> EditComment(int id, Comment editedComment)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null || comment.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                return NotFound();  // Ensure the user can only edit their own comment
            }

            comment.Content = editedComment.Content;
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> DeleteComment(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null || comment.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                return NotFound();  // Ensure the user can only delete their own comment
            }

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
