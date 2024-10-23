namespace NoteApp.Models
{
    public class Post
    {
        public int Id { get; set; }
        public required string Content { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public required string UserId { get; set; }
        public required string Username { get; set; }
        public List<Comment> Comments { get; set; } = new();
    }
}