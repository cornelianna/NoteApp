

namespace NoteApp.Models
{
    public class Post
{
    public int Id { get; set; }
    public string Content { get; set; }
    public string? ImageUrl { get; set; }
    public DateTime CreatedAt { get; set; }

    // Add these fields to link the post to a user
    public string UserId { get; set; }
    public string Username { get; set; }

    public List<Comment> Comments { get; set; } = new List<Comment>();
}

}