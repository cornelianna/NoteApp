using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using NoteApp.Models;
using System.Security.Claims;
using NoteApp.Repositories;
using Serilog;
using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;
using System;

namespace NoteApp.Controllers
{
    public class PostController : Controller
    {
        private readonly IPostRepository _postRepository;
        private readonly ICommentRepository _commentRepository;
        private readonly ILogger<PostController> _logger;

        public PostController(IPostRepository postRepository, ICommentRepository commentRepository, ILogger<PostController> logger)
        {
            _postRepository = postRepository;
            _commentRepository = commentRepository;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var posts = await _postRepository.GetAllPostsAsync();
                _logger.LogInformation("Fetched all posts");
                return View(posts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching all posts.");
                return RedirectToAction("Error");
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreatePost(Post post, IFormFile image)
        {
            try
            {
                if (post == null)
                {
                    _logger.LogWarning("Invalid post object provided.");
                    return BadRequest("Invalid post object.");
                }

                if (image != null && image.Length > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        await image.CopyToAsync(ms);
                        post.ImageData = ms.ToArray(); // Save image as byte array in the database
                    }
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null)
                {
                    _logger.LogWarning("User ID not found.");
                    return Forbid();
                }
                post.UserId = userId;
                post.Username = User.Identity?.Name ?? "Unknown";
                post.CreatedAt = DateTime.Now;

                await _postRepository.AddPostAsync(post);
                _logger.LogInformation("Post created successfully by user {UserId}", post.UserId);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a post.");
                return RedirectToAction("Error");
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> DeletePost(int id)
        {
            try
            {
                if (id <= 0)
                {
                    _logger.LogWarning("Invalid post ID provided for deletion: {PostId}", id);
                    return BadRequest("Invalid post ID.");
                }

                var post = await _postRepository.GetPostByIdAsync(id);
                if (post == null)
                {
                    _logger.LogWarning("Post not found for ID {PostId}", id);
                    return NotFound();
                }

                if (post.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
                {
                    _logger.LogWarning("User not authorized to delete post {PostId}", id);
                    return Forbid();
                }

                await _postRepository.DeletePostAsync(id);
                _logger.LogInformation("Post {PostId} deleted successfully by user {UserId}", id, User.FindFirstValue(ClaimTypes.NameIdentifier));
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting post {PostId}", id);
                return RedirectToAction("Error");
            }
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> UpdatePost(int id)
        {
            try
            {
                if (id <= 0)
                {
                    _logger.LogWarning("Invalid post ID provided for update: {PostId}", id);
                    return BadRequest("Invalid post ID.");
                }

                var post = await _postRepository.GetPostByIdAsync(id);
                if (post == null)
                {
                    _logger.LogWarning("Post not found for ID {PostId}", id);
                    return NotFound();
                }

                if (post.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
                {
                    _logger.LogWarning("User not authorized to update post {PostId}", id);
                    return Forbid();
                }

                return View(post);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching post {PostId} for update", id);
                return RedirectToAction("Error");
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UpdatePost(int id, Post updatedPost, IFormFile? newImage)
        {
            try
            {
                if (id <= 0 || updatedPost == null)
                {
                    _logger.LogWarning("Invalid post ID or updated post provided for update.");
                    return BadRequest("Invalid data.");
                }

                var post = await _postRepository.GetPostByIdAsync(id);
                if (post == null)
                {
                    _logger.LogWarning("Post not found for ID {PostId}", id);
                    return NotFound();
                }

                if (post.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
                {
                    _logger.LogWarning("User not authorized to update post {PostId}", id);
                    return Forbid();
                }

                if (newImage != null && newImage.Length > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        await newImage.CopyToAsync(ms);
                        post.ImageData = ms.ToArray(); // Save updated image as byte array
                    }
                }

                post.Content = updatedPost.Content;
                await _postRepository.UpdatePostAsync(post);
                _logger.LogInformation("Post {PostId} updated successfully by user {UserId}", id, post.UserId);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating post {PostId}", id);
                return RedirectToAction("Error");
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddComment(Comment comment, string returnUrl)
        {
            try
            {
                if (comment == null)
                {
                    _logger.LogWarning("Invalid comment object provided.");
                    return BadRequest("Invalid comment.");
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null)
                {
                    _logger.LogWarning("User ID not found.");
                    return Forbid();
                }
                comment.UserId = userId;
                comment.Username = User.Identity?.Name ?? "Unknown";
                comment.CreatedAt = DateTime.Now;

                await _commentRepository.AddCommentAsync(comment);
                _logger.LogInformation("Comment added successfully by user {UserId}", comment.UserId);
                
                // Redirect based on the returnUrl
                if (string.Equals(returnUrl, "ViewPost", StringComparison.OrdinalIgnoreCase))
                {
                    // Redirect back to the ViewPost action with the post ID
                    return RedirectToAction("ViewPost", new { id = comment.PostId });
                }
                else
                {
                    // Default redirection to Index
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding a comment.");
                return RedirectToAction("Error");
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> DeleteComment(int id)
        {
            try
            {
                if (id <= 0)
                {
                    _logger.LogWarning("Invalid comment ID provided for deletion: {CommentId}", id);
                    return BadRequest("Invalid comment ID.");
                }

                var comment = await _commentRepository.GetCommentByIdAsync(id);
                if (comment == null)
                {
                    _logger.LogWarning("Comment not found for ID {CommentId}", id);
                    return NotFound();
                }

                if (comment.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
                {
                    _logger.LogWarning("User not authorized to delete comment {CommentId}", id);
                    return Forbid();
                }

                await _commentRepository.DeleteCommentAsync(id);
                _logger.LogInformation("Comment {CommentId} deleted successfully by user {UserId}", id, User.FindFirstValue(ClaimTypes.NameIdentifier));
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting comment {CommentId}", id);
                return RedirectToAction("Error");
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UpdateComment(int id, Comment updatedComment)
        {
            try
            {
                if (id <= 0 || updatedComment == null)
                {
                    _logger.LogWarning("Invalid comment ID or updated comment provided for update.");
                    return BadRequest("Invalid data.");
                }

                var comment = await _commentRepository.GetCommentByIdAsync(id);
                if (comment == null)
                {
                    _logger.LogWarning("Comment not found for ID {CommentId}", id);
                    return NotFound();
                }

                if (comment.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
                {
                    _logger.LogWarning("User not authorized to update comment {CommentId}", id);
                    return Forbid();
                }

                comment.Content = updatedComment.Content;
                await _commentRepository.UpdateCommentAsync(comment);
                _logger.LogInformation("Comment {CommentId} updated successfully by user {UserId}", id, comment.UserId);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating comment {CommentId}", id);
                return RedirectToAction("Error");
            }
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> UpdateComment(int id)
        {
            try
            {
                if (id <= 0)
                {
                    _logger.LogWarning("Invalid comment ID provided for update: {CommentId}", id);
                    return BadRequest("Invalid comment ID.");
                }

                var comment = await _commentRepository.GetCommentByIdAsync(id);
                if (comment == null)
                {
                    _logger.LogWarning("Comment not found for ID {CommentId}", id);
                    return NotFound();
                }

                if (comment.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
                {
                    _logger.LogWarning("User not authorized to update comment {CommentId}", id);
                    return Forbid();
                }

                return View(comment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching comment {CommentId} for update", id);
                return RedirectToAction("Error");
            }
        }

        public async Task<IActionResult> ViewPost(int id)
        {
            try
            {
                if (id <= 0)
                {
                    _logger.LogWarning("Invalid post ID provided for viewing: {PostId}", id);
                    return BadRequest("Invalid post ID.");
                }

                var post = await _postRepository.GetPostByIdAsync(id);
                if (post == null)
                {
                    _logger.LogWarning("Post not found for ID {PostId}", id);
                    return NotFound();
                }

                return View(post);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while viewing post {PostId}", id);
                return RedirectToAction("Error");
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var exceptionHandlerPathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            if (exceptionHandlerPathFeature?.Error != null)
            {
                _logger.LogError(exceptionHandlerPathFeature.Error, "Unhandled exception occurred.");
            }

            var errorViewModel = new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier };
            return View(errorViewModel);
        }
    }
}
