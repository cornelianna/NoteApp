using Microsoft.AspNetCore.Mvc;
using NoteApp.Repositories;
using Microsoft.AspNetCore.Identity;
using Serilog;

namespace NoteApp.Controllers
{
    public class FriendController : Controller
    {
        private readonly IFriendRepository _friendRepository;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<FriendController> _logger;

        public FriendController(IFriendRepository friendRepository, UserManager<IdentityUser> userManager, ILogger<FriendController> logger)
        {
            _friendRepository = friendRepository;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                _logger.LogWarning("Current user is null. Redirecting to login.");
                // Optionally redirect to login
                return RedirectToAction("Login", "Account");
            }

            var friends = await _friendRepository.GetFriendsByUserIdAsync(currentUser.Id);
            _logger.LogInformation("Fetched friends for user {UserId}", currentUser.Id);
            return View(friends);
        }
    }
}