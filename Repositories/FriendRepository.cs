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

        public async Task<Friend> GetFriendshipAsync(string userId, string friendId)
        {
            _logger.LogInformation("Fetching friendship between {UserId} and {FriendId}", userId, friendId);
            return await _context.Friends
                .FirstOrDefaultAsync(f => f.UserId == userId && f.FriendId == friendId);
        }

        public async Task AddFriendAsync(Friend friendship)
        {
            await _context.Friends.AddAsync(friendship);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Added new friendship between {UserId} and {FriendId}", friendship.UserId, friendship.FriendId);
        }

        public async Task DeleteFriendAsync(string userId, string friendId)
        {
            var friendship = await _context.Friends
                .FirstOrDefaultAsync(f => f.UserId == userId && f.FriendId == friendId);
            if (friendship != null)
            {
                _context.Friends.Remove(friendship);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Deleted friendship between {UserId} and {FriendId}", userId, friendId);
            }
            else
            {
                _logger.LogWarning("Friendship between {UserId} and {FriendId} not found", userId, friendId);
            }
        }
    }
}
