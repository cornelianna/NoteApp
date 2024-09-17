using System.ComponentModel.DataAnnotations;

namespace NoteApp.Models
{
    public class Post
    {
        public int Id { get; set; }

        [MaxLength(1000)]
        public string? Content { get; set; }

        [MaxLength(500)]
        public string? ImageUrl { get; set; }

        public DateTime CreatedAt { get; set; }

        [MaxLength(50)]
        public string? UserId { get; set; }

        [MaxLength(50)]
        public string? Username { get; set; }

        public List<Comment> Comments { get; set; } = new List<Comment>();
    }
}