using NoteApp.Data;
using NoteApp.Models;
using Microsoft.EntityFrameworkCore;

namespace NoteApp.Repositories
{
public class PostRepository : IPostRepository
{
    private readonly NoteAppContext _context;

    public PostRepository(NoteAppContext context)
    {
        _context = context;
    }

    public async Task<Post> GetPostByIdAsync(int id)
    {
        return await _context.Posts.Include(p => p.Comments).FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<Post>> GetAllPostsAsync()
    {
        return await _context.Posts.OrderByDescending(post => post.CreatedAt).Include(post => post.Comments).ToListAsync();
    }

    public async Task AddPostAsync(Post post)
    {
        await _context.Posts.AddAsync(post);
        await _context.SaveChangesAsync();
    }

    public async Task UpdatePostAsync(Post post)
    {
        _context.Posts.Update(post);
        await _context.SaveChangesAsync();
    }

    public async Task DeletePostAsync(int id)
    {
        var post = await _context.Posts.FindAsync(id);
        if (post != null)
        {
            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
        }
    }
}
}
