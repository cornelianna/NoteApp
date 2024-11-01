using Microsoft.AspNetCore.Identity;

using System.ComponentModel.DataAnnotations.Schema;

namespace NoteApp.Models
{
    public class Friend
    {
        public int Id { get; set; }

        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public IdentityUser User { get; set; }

        public string FriendId { get; set; }
        [ForeignKey("FriendId")]
        public IdentityUser FriendUser { get; set; }
    }
}
