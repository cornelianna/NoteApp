using Microsoft.AspNetCore.Identity;
using NoteApp.Models;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System;

namespace NoteApp.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<AccountRepository> _logger;

        public AccountRepository(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, ILogger<AccountRepository> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        public async Task<IdentityResult> RegisterAsync(RegisterViewModel model)
        {
            try
            {
                var user = new IdentityUser { UserName = model.Username, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    _logger.LogInformation("User registered and signed in successfully with username: {Username}", model.Username);
                }
                else
                {
                    _logger.LogWarning("User registration failed for username: {Username}. Errors: {Errors}", model.Username, string.Join(", ", result.Errors));
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during registration for user: {Username}", model.Username);
                throw; // Re-throwing the exception to let the controller handle it if needed
            }
        }

        public async Task<SignInResult> LoginAsync(LoginViewModel model)
        {
            try
            {
                var result = await _signInManager.PasswordSignInAsync(
                    model.Username, model.Password, model.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in successfully with username: {Username}", model.Username);
                }
                else
                {
                    _logger.LogWarning("Failed login attempt for username: {Username}", model.Username);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during login for user: {Username}", model.Username);
                throw; // Re-throwing the exception to let the controller handle it if needed
            }
        }

        public async Task LogoutAsync()
        {
            try
            {
                await _signInManager.SignOutAsync();
                _logger.LogInformation("User logged out successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during logout.");
                throw; // Re-throwing the exception to let the controller handle it if needed
            }
        }
    }
}
