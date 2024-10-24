using System.ComponentModel.DataAnnotations;

namespace NoteApp.Models 
{
    public class Comment 
    {
        public int Id { get; set; } 
        public int PostId { get; set; } 
        
        [MaxLength(500)]
        public string? Content { get; set; }
        public DateTime CreatedAt { get; set; }
        
        [MaxLength(50)]
        public string? UserId { get; set; }
        [MaxLength(50)]
        public string? Username { get; set; } 

        public Post? Post { get; set; } 
    }
}