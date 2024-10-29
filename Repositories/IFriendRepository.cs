using NoteApp.Models;
<<<<<<< HEAD
=======

>>>>>>> origin/anna-test
namespace NoteApp.Repositories
{
    public interface IFriendRepository
    {
        Task<IEnumerable<Friend>> GetFriendsByUserIdAsync(string userId);
<<<<<<< HEAD
        Task<Friend> GetFriendByIdAsync(int id);
        Task AddFriendAsync(Friend friendship);
        Task UpdateFriendAsync(Friend friendship);
        Task DeleteFriendAsync(int id);
    }
}
=======
        Task<Friend> GetFriendshipAsync(string userId, string friendId);
        Task AddFriendAsync(Friend friendship);
        Task DeleteFriendAsync(string userId, string friendId);
    }
}
>>>>>>> origin/anna-test
