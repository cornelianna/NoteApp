using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NoteApp.Models 
{
    public class Comment 
    {
        public Comment() { }
        // Navigation property to Post
        [ForeignKey("PostId")]
        public Post Post { get; set; } = null!;
        // Constructor with scalar properties
        public Comment(int postId, string content, string userId)
        {
            PostId = postId;
            Content = content;
            UserId = userId;
            CreatedAt = DateTime.UtcNow;
        }
        public int Id { get; set; } 

        [Required(ErrorMessage = "Post ID is required.")]
        public int PostId { get; set; } 
        
        [Required(ErrorMessage = "Content is required.")]
        [MaxLength(500, ErrorMessage = "Content cannot exceed 500 characters.")]
        public string Content { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        [Required(ErrorMessage = "User ID is required.")]
        public string UserId { get; set; } = string.Empty;

        [ForeignKey("UserId")]
        public User User { get; set; } = null!;

        [Required(ErrorMessage = "Username is required.")]
        [MaxLength(50, ErrorMessage = "Username cannot exceed 50 characters.")]
        public string Username { get; set; } = string.Empty;
    }
}
