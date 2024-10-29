using NoteApp.Models;
using Microsoft.EntityFrameworkCore;
using NoteApp.Data;
using Serilog;

namespace NoteApp.Repositories
{
    public class FriendRepository : IFriendRepository
    {
        private readonly NoteAppContext _context;
        private readonly ILogger<FriendRepository> _logger;

        public FriendRepository(NoteAppContext context, ILogger<FriendRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Friend>> GetFriendsByUserIdAsync(string userId)
        {
            _logger.LogInformation("Fetching friends for user {UserId}", userId);
            return await _context.Friends
                .Include(f => f.FriendUser)
                .Where(f => f.UserId == userId)
                .ToListAsync();
        }

        public async Task<Friend> GetFriendByIdAsync(int id)
        {
            _logger.LogInformation("Fetching friend with ID {Id}", id);
            return await _context.Friends.FindAsync(id);
        }

        public async Task AddFriendAsync(Friend friend)
        {
            await _context.Friends.AddAsync(friend);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Added new friend with ID {Id}", friend.Id);
        }

        public async Task UpdateFriendAsync(Friend friend)
        {
            _context.Friends.Update(friend);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Updated friend with ID {Id}", friend.Id);
        }

        public async Task DeleteFriendAsync(int id)
        {
            var friend = await _context.Friends.FindAsync(id);
            if (friend != null)
            {
                _context.Friends.Remove(friend);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Deleted friend with ID {Id}", id);
            }
            else
            {
                _logger.LogWarning("Friend with ID {Id} not found", id);
            }
        }
    }
}