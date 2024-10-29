using NoteApp.Models;
namespace NoteApp.Repositories
{
    public interface IFriendRepository
    {
        Task<IEnumerable<Friend>> GetFriendsByUserIdAsync(string userId);
        Task<Friend> GetFriendByIdAsync(int id);
        Task AddFriendAsync(Friend friendship);
        Task UpdateFriendAsync(Friend friendship);
        Task DeleteFriendAsync(int id);
    }
}