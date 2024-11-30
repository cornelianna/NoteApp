using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace NoteApp.Models
{
    public class User : IdentityUser // IdentityUser includes username and password properties. User inherits from IdentityUser
    {
        [MaxLength(200, ErrorMessage = "Profile picture URL cannot exceed 200 characters.")]
        [Url(ErrorMessage = "The profile picture URL must be a valid URL.")]
        public string? ProfilePictureUrl { get; set; }

        // Navigation property for related posts
        public List<Post> Posts { get; set; } = new();
        public ICollection<Friend> Friends { get; set; } = new List<Friend>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}
