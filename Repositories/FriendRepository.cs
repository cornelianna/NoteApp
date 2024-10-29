using NoteApp.Models;
using Microsoft.EntityFrameworkCore;
using NoteApp.Data;

namespace NoteApp.Repositories
{
    public class FriendRepository : IFriendRepository
    {
        private readonly NoteAppContext _context;

        public FriendRepository(NoteAppContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Friend>> GetFriendsByUserIdAsync(string userId)
        {
            return await _context.Friends
                .Include(f => f.FriendUser)
                .Where(f => f.UserId == userId)
                .ToListAsync();
        }

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
