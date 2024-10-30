using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using NoteApp.Models;
using Serilog;
using NoteApp.Repositories;

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
                // Redirect to login if user is not authenticated
                _logger.LogWarning("User is not authenticated. Redirecting to login page.");
                return RedirectToAction("Login", "Account");
            }

            var friends = await _friendRepository.GetFriendsByUserIdAsync(currentUser.Id);
            _logger.LogInformation("Fetched friends for user {UserId}", currentUser.Id);
            return View(friends);
        }

        // Add this action to search for users
        [HttpPost]
        public async Task<IActionResult> SearchUsers(string searchTerm)
        {
            var users = await _userManager.Users
                .Where(u => u.UserName.Contains(searchTerm) || u.Email.Contains(searchTerm))
                .ToListAsync();
            
            _logger.LogInformation("Fetched users for search term {SearchTerm}", searchTerm);
            return PartialView("_UserSearchResults", users);
        }

        // Add this action to add a friend
        [HttpPost]
        public async Task<IActionResult> AddFriend(string friendId)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                _logger.LogWarning("Current user is null. Redirecting to login.");
                return Json(new { success = false, message = "User not logged in." });
            }

            if (currentUser.Id == friendId)
            {
                _logger.LogWarning("User tried to add themselves as a friend.");
                return Json(new { success = false, message = "You cannot add yourself as a friend." });
            }

            var existingFriendship = await _friendRepository.GetFriendshipAsync(currentUser.Id, friendId);
            if (existingFriendship != null)
            {
                _logger.LogWarning("User tried to add a friend they are already friends with.");
                return Json(new { success = false, message = "You are already friends with this user." });
            }

            var friendship = new Friend
            {
                UserId = currentUser.Id,
                FriendId = friendId
            };

            await _friendRepository.AddFriendAsync(friendship);
            _logger.LogInformation("User {UserId} added friend {FriendId}", currentUser.Id, friendId);
            return Json(new { success = true, message = "Friend added successfully." });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteFriend(string friendId)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                _logger.LogWarning("Current user is null. Redirecting to login.");
                return Json(new { success = false, message = "User not logged in." });
            }

            await _friendRepository.DeleteFriendAsync(currentUser.Id, friendId);
            _logger.LogInformation("User {UserId} deleted friend {FriendId}", currentUser.Id, friendId);
            return Json(new { success = true, message = "Friend deleted successfully." });
        }
    }
}
