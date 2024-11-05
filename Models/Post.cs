using System.ComponentModel.DataAnnotations;

namespace NoteApp.Models
{
    public class Post
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Content is required.")]
        public string Content { get; set; } = string.Empty;
        public byte[]? ImageData { get; set; } // Image stored as byte array
        public DateTime CreatedAt { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public List<Comment> Comments { get; set; } = new();
    }
}
