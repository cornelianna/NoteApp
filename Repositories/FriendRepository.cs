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

        public async Task<Friend> GetFriendByIdAsync(int id)
        {
            return await _context.Friends.FindAsync(id);
        }

        public async Task AddFriendAsync(Friend friend)
        {
            await _context.Friends.AddAsync(friend);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateFriendAsync(Friend friend)
        {
            _context.Friends.Update(friend);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteFriendAsync(int id)
        {
            var friend = await _context.Friends.FindAsync(id);
            if (friend != null)
            {
                _context.Friends.Remove(friend);
                await _context.SaveChangesAsync();
            }
        }
    }
}