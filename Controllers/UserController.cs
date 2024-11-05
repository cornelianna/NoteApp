// UserController.cs
using Microsoft.AspNetCore.Mvc;
using NoteApp.Models;
using NoteApp.Repositories;
using System.Threading.Tasks;

namespace NoteApp.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IPostRepository _postRepository;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserRepository userRepository, IPostRepository postRepository, ILogger<UserController> logger)
        {
            _userRepository = userRepository;
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

            var user = await _userRepository.GetUserAsync(User);
            if (user == null)
            {
                _logger.LogWarning("User not found.");
                return RedirectToAction("Login", "Account");
            }

            // Update username
            if (user.UserName != model.Username)
            {
                var setUsernameResult = await _userRepository.SetUserNameAsync(user, model.Username);
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
                var setEmailResult = await _userRepository.SetEmailAsync(user, model.Email);
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
                var changePasswordResult = await _userRepository.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
                if (!changePasswordResult.Succeeded)
                {
                    foreach (var error in changePasswordResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(model);
                }
            }

            await _userRepository.RefreshSignInAsync(user);
            return RedirectToAction("Index", "Post");
        }

        // View any user's profile
        public async Task<IActionResult> Profile(string? userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                // If no userId is provided, show the current user's profile
                userId = await _userRepository.GetUserIdAsync(User);
                if (userId == null)
                {
                    _logger.LogWarning("No user is logged in and no userId was provided.");
                    return RedirectToAction("Login", "Account");
                }
            }

            var user = await _userRepository.FindByIdAsync(userId);
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