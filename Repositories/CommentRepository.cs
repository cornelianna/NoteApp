using NoteApp.Data;
using NoteApp.Models;
using Serilog;

namespace NoteApp.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private readonly NoteAppContext _context;
        private readonly ILogger<CommentRepository> _logger;

        public CommentRepository(NoteAppContext context, ILogger<CommentRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Comment> GetCommentByIdAsync(int id)
        {
            _logger.LogInformation("Fetching comment with ID {Id}", id);
            return await _context.Comments.FindAsync(id);
        }

        public async Task AddCommentAsync(Comment comment)
        {
            await _context.Comments.AddAsync(comment);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Added new comment with ID {Id}", comment.Id);
        }

        public async Task UpdateCommentAsync(Comment comment)
        {
            _context.Comments.Update(comment);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Updated comment with ID {Id}", comment.Id);
        }

        public async Task DeleteCommentAsync(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment != null)
            {
                _context.Comments.Remove(comment);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Deleted comment with ID {Id}", id);
            }
            else
            {
                _logger.LogWarning("Comment with ID {Id} not found", id);
            }
        }
    }
}