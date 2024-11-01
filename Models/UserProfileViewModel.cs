namespace NoteApp.Models
{
    public class UserProfileViewModel
    {
        public string UserId { get; set; }
        public string Username { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public List<Post> Posts { get; set; } = new();
    }
}