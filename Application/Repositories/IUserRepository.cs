// IUserRepository.cs
using NoteApp.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace NoteApp.Repositories
{
    public interface IUserRepository
    {
        Task<User> GetUserAsync(ClaimsPrincipal principal);
        Task<string> GetUserIdAsync(ClaimsPrincipal principal);
        Task<User> FindByIdAsync(string userId);
        Task<IdentityResult> SetUserNameAsync(User user, string userName);
        Task<IdentityResult> SetEmailAsync(User user, string email);
        Task<IdentityResult> ChangePasswordAsync(User user, string currentPassword, string newPassword);
        Task RefreshSignInAsync(User user);
    }
}