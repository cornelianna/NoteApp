using NoteApp.Models;
using NoteApp.Repositories;
using Microsoft.EntityFrameworkCore;
using NoteApp.Data;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace NoteApp.Repositories
{
    public class FriendRepository : IFriendRepository
    {
        private readonly NoteAppContext _context;

        public FriendRepository(NoteAppContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<Friend>> GetFriendsByUserIdAsync(string userId)
        {
            if (_context.Friends == null)
            {
                throw new InvalidOperationException("Friends collection is not initialized.");
            }

            return await _context.Friends
                .Where(f => f.UserId == userId)
                .ToListAsync();
        }

        public async Task<Friend?> GetFriendshipAsync(string userId, string friendId)
        {
            if (_context.Friends == null)
            {
                throw new InvalidOperationException("Friends collection is not initialized.");
            }

            return await _context.Friends
                .FirstOrDefaultAsync(f => f.UserId == userId && f.FriendId == friendId);
        }

        public async Task AddFriendAsync(Friend friendship)
        {
            if (_context.Friends == null)
            {
                throw new InvalidOperationException("Friends collection is not initialized.");
            }

            await _context.Friends.AddAsync(friendship);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteFriendAsync(string userId, string friendId)
        {
            if (_context.Friends == null)
            {
                throw new InvalidOperationException("Friends collection is not initialized.");
            }

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
