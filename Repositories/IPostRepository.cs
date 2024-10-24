using NoteApp.Models;

namespace NoteApp.Repositories
{

public interface IPostRepository
{
    Task<Post> GetPostByIdAsync(int id);
    Task<IEnumerable<Post>> GetAllPostsAsync();
    Task AddPostAsync(Post post);
    Task UpdatePostAsync(Post post);
    Task DeletePostAsync(int id);
}

public interface ICommentRepository
{
    Task<Comment> GetCommentByIdAsync(int id);
    Task AddCommentAsync(Comment comment);
    Task UpdateCommentAsync(Comment comment);
    Task DeleteCommentAsync(int id);
}
}