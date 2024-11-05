// UserRepository.cs
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NoteApp.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public UserRepository(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IdentityUser> GetUserAsync(ClaimsPrincipal principal)
        {
            return await _userManager.GetUserAsync(principal);
        }

        public Task<string> GetUserIdAsync(ClaimsPrincipal principal)
        {
            return Task.FromResult(_userManager.GetUserId(principal));
        }

        public async Task<IdentityUser> FindByIdAsync(string userId)
        {
            return await _userManager.FindByIdAsync(userId);
        }

        public async Task<IdentityResult> SetUserNameAsync(IdentityUser user, string userName)
        {
            return await _userManager.SetUserNameAsync(user, userName);
        }

        public async Task<IdentityResult> SetEmailAsync(IdentityUser user, string email)
        {
            return await _userManager.SetEmailAsync(user, email);
        }

        public async Task<IdentityResult> ChangePasswordAsync(IdentityUser user, string currentPassword, string newPassword)
        {
            return await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        }

        public async Task RefreshSignInAsync(IdentityUser user)
        {
            await _signInManager.RefreshSignInAsync(user);
        }
    }
}