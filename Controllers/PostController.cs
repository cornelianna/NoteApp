using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using NoteApp.Models;
using NoteApp.Data;
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
            var posts = await _context.Posts
                .OrderByDescending(post => post.CreatedAt)
                .Include(post => post.Comments)
                .ToListAsync();
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
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var username = User.Identity?.Name;

            if (userId == null || username == null)
            {
                // Handle the case where the user is not authenticated properly
                return Unauthorized();
            }

            post.UserId = userId;
            post.Username = username;

            post.CreatedAt = DateTime.Now;
            _context.Add(post);
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
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var username = User.Identity?.Name;

                if (userId == null || username == null)
                {
                    // Handle the case where the user is not authenticated properly
                    return Unauthorized();
                }

                comment.UserId = userId;
                comment.Username = username;

                comment.PostId = post.Id;
                comment.CreatedAt = DateTime.Now;
                _context.Comments.Add(comment);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> EditPost(int id)
        {
            var post = await _context.Posts.FindAsync(id);

            if (post == null)
            {
                return NotFound();
            }
            
            if (post.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                return Forbid();  
            }

            return View(post);  
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> EditPost(int id, Post updatedPost, IFormFile? newImage)
        {
            var post = await _context.Posts.FindAsync(id);

            if (post == null)
            {
                return NotFound();
            }

            
            if (post.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                return Forbid();
            }

            if (newImage != null && newImage.Length > 0)
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", newImage.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await newImage.CopyToAsync(stream);
                }
                post.ImageUrl = "/uploads/" + newImage.FileName;
            }

            post.Content = updatedPost.Content;
            post.CreatedAt = DateTime.Now;  

            _context.Update(post);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> DeletePost(int id)
        {
            var post = await _context.Posts.FindAsync(id);

            if (post == null)
            {
                return NotFound();
            }

            // Ensure that only the user who created the post can delete it
            if (post.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                return Forbid();  // Return a 403 Forbidden response if the user is not the owner
            }

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> EditComment(int id)
        {
            var comment = await _context.Comments.FindAsync(id);

            if (comment == null)
            {
                return NotFound();
            }

            if (comment.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                return Forbid();
            }

            return View(comment);  // Return the comment for editing
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> EditComment(int id, Comment updatedComment)
        {
            var comment = await _context.Comments.FindAsync(id);

            if (comment == null)
            {
                return NotFound();
            }

            if (comment.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                return Forbid();
            }

            comment.Content = updatedComment.Content;

            _context.Update(comment);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }


        [Authorize]
        [HttpPost]
        public async Task<IActionResult> DeleteComment(int id)
        {
            var comment = await _context.Comments.FindAsync(id);

            if (comment == null)
            {
                return NotFound();
            }

            if (comment.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                return Forbid();
            }

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

            }
        }
