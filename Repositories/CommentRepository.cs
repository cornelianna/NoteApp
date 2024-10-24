using NoteApp.Data;
using NoteApp.Models;

namespace NoteApp.Repositories
{
public class CommentRepository : ICommentRepository
{
    private readonly NoteAppContext _context;

    public CommentRepository(NoteAppContext context)
    {
        _context = context;
    }

    public async Task<Comment> GetCommentByIdAsync(int id)
    {
        return await _context.Comments.FindAsync(id);
    }

    public async Task AddCommentAsync(Comment comment)
    {
        await _context.Comments.AddAsync(comment);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateCommentAsync(Comment comment)
    {
        _context.Comments.Update(comment);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteCommentAsync(int id)
    {
        var comment = await _context.Comments.FindAsync(id);
        if (comment != null)
        {
            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
        }
    }
}
}