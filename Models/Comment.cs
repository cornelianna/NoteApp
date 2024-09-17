using System.ComponentModel.DataAnnotations;

namespace NoteApp.Models //namespace for comments
{
    public class Comment //class for comments
    {
        public int Id { get; set; } //getters and setters
        public int PostId { get; set; } 
        
        [MaxLength(500)]
        public string? Content { get; set; }
        public System.DateTime CreatedAt { get; set; }

        
        [MaxLength(50)]
        public string? UserId { get; set; }
        [MaxLength(50)]
        public string? Username { get; set; } 

        public Post? Post { get; set; } 
    }
}