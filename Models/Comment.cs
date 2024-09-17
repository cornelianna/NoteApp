namespace NoteApp.Models
{
    public class Comment
{
    public int Id { get; set; }
    public int PostId { get; set; }  // Foreign key to the post
    public string Content { get; set; }
    public DateTime CreatedAt { get; set; }

    // Add these fields to link the comment to a user
    public string UserId { get; set; }
    public string Username { get; set; }

    public Post Post { get; set; }
}

}
