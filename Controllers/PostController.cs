using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using NoteApp.Models;
using System.Security.Claims;
using NoteApp.Repositories;
using Serilog;

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
        var posts = await _postRepository.GetAllPostsAsync();
        _logger.LogInformation("Fetched all posts");
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

        post.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        post.Username = User.Identity?.Name;
        post.CreatedAt = DateTime.Now;

        await _postRepository.AddPostAsync(post);
        _logger.LogInformation("Post created successfully.");
        return RedirectToAction("Index");
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> DeletePost(int id)
    {
        await _postRepository.DeletePostAsync(id);
        _logger.LogInformation("Post deleted successfully.");
        return RedirectToAction("Index");
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> UpdatePost(int id)
    {
        var post = await _postRepository.GetPostByIdAsync(id);

        if (post == null)
        {
            return NotFound();
        }
        
        if (post.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
        {
            _logger.LogWarning("User not authorized to update this post.");
            return Forbid();  
        }
        return View(post);  
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> UpdatePost(int id, Post updatedPost, IFormFile? newImage)
    {
        var post = await _postRepository.GetPostByIdAsync(id);
            
            if (newImage != null && newImage.Length > 0)
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", newImage.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await newImage.CopyToAsync(stream);
                }
                updatedPost.ImageUrl = "/uploads/" + newImage.FileName;
            }
        post.ImageUrl = updatedPost.ImageUrl;
        post.Content = updatedPost.Content;
        await _postRepository.UpdatePostAsync(post);
        _logger.LogInformation("Post updated successfully.");
        return RedirectToAction("Index");
    }
    
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> AddComment(Comment comment)
    {
        comment.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        comment.Username = User.Identity?.Name;
        comment.CreatedAt = DateTime.Now;

        await _commentRepository.AddCommentAsync(comment);
        _logger.LogInformation("Comment added successfully.");
        return RedirectToAction("Index");
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> DeleteComment(int id)
    {
        await _commentRepository.DeleteCommentAsync(id);
        _logger.LogInformation("Comment deleted successfully.");
        return RedirectToAction("Index");
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> UpdateComment(int id)
    {
        var comment = await _commentRepository.GetCommentByIdAsync(id);

        if (comment == null)
        {
            return NotFound();
        }

        if (comment.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
        {
            _logger.LogWarning("User not authorized to update this comment.");
            return Forbid();
        }

        return View(comment); 
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> UpdateComment(int id, Comment updatedComment)
    {   
        var comment = await _commentRepository.GetCommentByIdAsync(id);
        
        comment.Content = updatedComment.Content;
        _logger.LogInformation("Comment updated successfully.");
        await _commentRepository.UpdateCommentAsync(comment);
        return RedirectToAction("Index");
        
    }

    public async Task<IActionResult> ViewPost(int id)
    {
        var post = await _postRepository.GetPostByIdAsync(id);
        if (post == null)
        {
            return NotFound();
        }

        return View(post);
    }

}
}
