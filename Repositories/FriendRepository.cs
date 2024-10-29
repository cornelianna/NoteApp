using NoteApp.Models;
using Microsoft.EntityFrameworkCore;
using NoteApp.Data;
<<<<<<< HEAD
using Serilog;
=======
>>>>>>> origin/anna-test

namespace NoteApp.Repositories
{
    public class FriendRepository : IFriendRepository
    {
        private readonly NoteAppContext _context;
<<<<<<< HEAD
        private readonly ILogger<FriendRepository> _logger;

        public FriendRepository(NoteAppContext context, ILogger<FriendRepository> logger)
        {
            _context = context;
            _logger = logger;
=======

        public FriendRepository(NoteAppContext context)
        {
            _context = context;
>>>>>>> origin/anna-test
        }

        public async Task<IEnumerable<Friend>> GetFriendsByUserIdAsync(string userId)
        {
<<<<<<< HEAD
            _logger.LogInformation("Fetching friends for user {UserId}", userId);
=======
>>>>>>> origin/anna-test
            return await _context.Friends
                .Include(f => f.FriendUser)
                .Where(f => f.UserId == userId)
                .ToListAsync();
        }

<<<<<<< HEAD
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
=======
        public async Task<Friend> GetFriendshipAsync(string userId, string friendId)
        {
            return await _context.Friends
                .FirstOrDefaultAsync(f => f.UserId == userId && f.FriendId == friendId);
        }

        public async Task AddFriendAsync(Friend friendship)
        {
            await _context.Friends.AddAsync(friendship);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteFriendAsync(string userId, string friendId)
        {
            var friendship = await _context.Friends
                .FirstOrDefaultAsync(f => f.UserId == userId && f.FriendId == friendId);
            if (friendship != null)
            {
                _context.Friends.Remove(friendship);
                await _context.SaveChangesAsync();
            }
        }
    }
}
>>>>>>> origin/anna-test
