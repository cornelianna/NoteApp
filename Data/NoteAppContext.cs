// Data/NoteAppContext.cs
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NoteApp.Models;

    public class NoteAppContext : IdentityDbContext
    {
        public NoteAppContext(DbContextOptions<NoteAppContext> options)
            : base(options)
        {
        }

        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
    }
    