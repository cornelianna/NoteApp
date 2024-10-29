using NoteApp.Data;
using NoteApp.Models;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace NoteApp.Repositories
{
    public class PostRepository : IPostRepository
    {
        private readonly NoteAppContext _context;
        private readonly ILogger<PostRepository> _logger;

        public PostRepository(NoteAppContext context, ILogger<PostRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Post> GetPostByIdAsync(int id)
        {
            _logger.LogInformation("Fetching post with ID {Id}", id);
            return await _context.Posts
                .Include(p => p.Comments)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Post>> GetAllPostsAsync()
        {
            _logger.LogInformation("Fetching all posts");
            return await _context.Posts.OrderByDescending(post => post.CreatedAt).Include(post => post.Comments).ToListAsync();
        }

        public async Task AddPostAsync(Post post)
        {
            await _context.Posts.AddAsync(post);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Added new post with ID {Id}", post.Id);
        }

        public async Task UpdatePostAsync(Post post)
        {
            _context.Posts.Update(post);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Updated post with ID {Id}", post.Id);
        }

        public async Task DeletePostAsync(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post != null)
            {
                _context.Posts.Remove(post);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Deleted post with ID {Id}", id);
            }
            else
            {
                _logger.LogWarning("Post with ID {Id} not found", id);
            }
        }
    }
}