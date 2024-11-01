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
        private readonly IPostRepository _postRepository;
        private readonly ILogger<UserController> _logger;

        public UserController(UserManager<IdentityUser> userManager, IPostRepository postRepository, ILogger<UserController> logger)
        {
            _userManager = userManager;
            _postRepository = postRepository;
            _logger = logger;
        }



   
        [Authorize]
        public IActionResult Settings()
        {
            _logger.LogInformation("Accessed Settings page.");
            var model = new EditProfileViewModel();
            return View(model);
        }

        [HttpPost]
        public IActionResult UpdateProfile(EditProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for UpdateProfile.");
                return View("Settings", model);
            }
            try
            {
                // Update profile logic here
                _logger.LogInformation("Profile updated successfully.");
                return RedirectToAction("Settings");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating profile.");
                ModelState.AddModelError(string.Empty, "An error occurred while updating your profile. Please try again later.");
                return View("Settings", model);
            }
        }

          // View any user's profile
        public async Task<IActionResult> Profile(string userId)
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
