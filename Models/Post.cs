

namespace NoteApp.Models
{
public class Post
{
    public int Id { get; set; }
    public string Content { get; set; }
    public string? ImageUrl { get; set; }  // Allow ImageUrl to be null
    public DateTime CreatedAt { get; set; }
    public string UserId { get; set; }
    public string Username { get; set; }
    public List<Comment> Comments { get; set; } = new List<Comment>();
}

}