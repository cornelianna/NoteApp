using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NoteApp.Models;

namespace NoteApp.Data
{
    public class NoteAppContext : IdentityDbContext<User>
    {
        public NoteAppContext(DbContextOptions<NoteAppContext> options)
            : base(options)
        {
            Posts = Set<Post>();
            Comments = Set<Comment>();
            Friends = Set<Friend>();
        }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Friend> Friends { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            SeedUsers(builder);

            builder.Entity<Friend>()
                .HasOne(f => f.User)
                .WithMany()
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Friend>()
                .HasOne(f => f.FriendUser)
                .WithMany()
                .HasForeignKey(f => f.FriendId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        private static void SeedUsers(ModelBuilder builder)
        {
            var hasher = new PasswordHasher<User>();

            var user1 = new User
            {
                Id = "user1-id",
                UserName = "user1",
                NormalizedUserName = "USER1",
                Email = "user1@example.com",
                NormalizedEmail = "USER1@EXAMPLE.COM",
                EmailConfirmed = true,
                PasswordHash = hasher.HashPassword(new User(), "Password123!")
            };

            var user2 = new IdentityUser
            {
                Id = "user2-id",
                UserName = "user2",
                NormalizedUserName = "USER2",
                Email = "user2@example.com",
                NormalizedEmail = "USER2@EXAMPLE.COM",
                EmailConfirmed = true,
                PasswordHash = hasher.HashPassword(new User(), "Password123!")
            };

            builder.Entity<IdentityUser>().HasData(user1, user2);
        }
    }
}
