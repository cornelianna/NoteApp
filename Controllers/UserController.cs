using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NoteApp.Models;
using NoteApp.Repositories;
using Serilog;

namespace NoteApp.Controllers
{
    public class UserController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IPostRepository _postRepository;
        private readonly ILogger<UserController> _logger;

        public UserController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IPostRepository postRepository, ILogger<UserController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _postRepository = postRepository;
            _logger = logger;
        }
        
        public IActionResult Settings()
        {
            _logger.LogInformation("Accessed Settings page.");
            var model = new EditProfileViewModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Settings(EditProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                _logger.LogWarning("User not found.");
                return RedirectToAction("Login", "Account");
            }

            // Update username
            if (user.UserName != model.Username)
            {
                var setUsernameResult = await _userManager.SetUserNameAsync(user, model.Username);
                if (!setUsernameResult.Succeeded)
                {
                    foreach (var error in setUsernameResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(model);
                }
            }

            // Update email
            if (user.Email != model.Email)
            {
                var setEmailResult = await _userManager.SetEmailAsync(user, model.Email);
                if (!setEmailResult.Succeeded)
                {
                    foreach (var error in setEmailResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(model);
                }
            }

            // Change password
            if (!string.IsNullOrEmpty(model.NewPassword))
            {
                var changePasswordResult = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
                if (!changePasswordResult.Succeeded)
                {
                    foreach (var error in changePasswordResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(model);
                }
            }

            await _signInManager.RefreshSignInAsync(user);
            return RedirectToAction("Index", "Post");
        }

        // View any user's profile
        public async Task<IActionResult> Profile(string? userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                // If no userId is provided, show the current user's profile
                userId = _userManager.GetUserId(User);
                if (userId == null)
                {
                    _logger.LogWarning("No user is logged in and no userId was provided.");
                    return RedirectToAction("Login", "Account");
                }
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found.", userId);
                return NotFound("User not found.");
            }

            var posts = await _postRepository.GetPostsByUserIdAsync(userId);

            var model = new UserProfileViewModel
            {
                UserId = user.Id,
                Username = user.UserName,
                ProfilePictureUrl = "/images/default-profile.png", // Update this if you have profile pictures
                Posts = posts.ToList()
            };

            _logger.LogInformation("Displaying profile for user {UserId}.", userId);
            return View(model);
        }
    }
}