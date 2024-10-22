using Microsoft.AspNetCore.Identity;

namespace NoteApp.Models
{
    public class Friendship
    {
        public required string UserId { get; set; }
        public required IdentityUser User { get; set; }

        public required string FriendId { get; set; }
        public required IdentityUser Friend { get; set; }
    }
}