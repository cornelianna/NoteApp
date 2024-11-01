using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NoteApp.Models;
using NoteApp.Repositories;
using NoteApp.Repositories;
using Serilog;

namespace NoteApp.Controllers
{
    public class UserController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IPostRepository _postRepository;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IPostRepository _postRepository;
        private readonly ILogger<UserController> _logger;

        public UserController(UserManager<IdentityUser> userManager, IPostRepository postRepository, UserManager<IdentityUser> userManager, IPostRepository postRepository, ILogger<UserController> logger)
        {
            _userManager = userManager;
            _postRepository = postRepository;
            _userManager = userManager;
            _postRepository = postRepository;
            _logger = logger;
        }

        // View any user's profile
        public async Task<IActionResult> Profile(string userId)
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
    }
}