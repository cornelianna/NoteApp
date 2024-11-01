namespace NoteApp.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string? ProfilePictureUrl { get; set; }
    }
}