using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Authorization;
using NoteApp.Models;
using System.Security.Claims;
using NoteApp.Repositories;

namespace NoteApp.Controllers
{
    public class PostController : Controller
{
    private readonly IPostRepository _postRepository;
    private readonly ICommentRepository _commentRepository;

    public PostController(IPostRepository postRepository, ICommentRepository commentRepository)
    {
        _postRepository = postRepository;
        _commentRepository = commentRepository;
        }
    

    public async Task<IActionResult> Index()
    {
        var posts = await _postRepository.GetAllPostsAsync();
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
        return RedirectToAction("Index");
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> DeletePost(int id)
    {
        await _postRepository.DeletePostAsync(id);
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
        return RedirectToAction("Index");
    }
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> DeleteComment(int id)
    {
        await _commentRepository.DeleteCommentAsync(id);
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
        await _commentRepository.UpdateCommentAsync(comment);
        return RedirectToAction("Index");
        
    }
}
}
