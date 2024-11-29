using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace NoteApp.Models
{
    public class Friend
    {   
        public Friend()
        {
        }
        [SetsRequiredMembers]
        public Friend(string userId, string friendId)
        {
            UserId = userId;
            FriendId = friendId;
        }
        public int Id { get; set; }

        [Required(ErrorMessage = "User ID is required.")]
        [MaxLength(50, ErrorMessage = "User ID cannot exceed 50 characters.")]
        public string? UserId { get; set; }

        [Required(ErrorMessage = "Friend ID is required.")]
        [MaxLength(50, ErrorMessage = "Friend ID cannot exceed 50 characters.")]
        public string? FriendId { get; set; }

        // Navigation property to User
        [ForeignKey("UserId")]
        public User User { get; set; } = null!; // required to be initialized during creation

        // Navigation property to FriendUser
        [ForeignKey("FriendId")]
        public required User FriendUser { get; set; } = null!; // required to be initialized during creation
        
    }
}