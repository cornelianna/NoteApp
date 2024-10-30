using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NoteApp.Models;
using Serilog;

namespace NoteApp.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly ILogger<UserController> _logger;

        public UserController(ILogger<UserController> logger)
        {
            _logger = logger;
        }
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
    }
}