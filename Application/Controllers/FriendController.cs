using NoteApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using NoteApp.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace NoteApp.Controllers
{
    [Authorize]
    public class FriendController : Controller
    {
        private readonly IFriendRepository _friendRepository;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<FriendController> _logger;

        public FriendController(IFriendRepository friendRepository, UserManager<User> userManager, ILogger<FriendController> logger)
        {
            _friendRepository = friendRepository;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser == null)
                {
                    _logger.LogWarning("User is not authenticated. Redirecting to login page.");
                    return RedirectToAction("Login", "Account");
                }

                var friends = await _friendRepository.GetFriendsByUserIdAsync(currentUser.Id);
                _logger.LogInformation("Fetched {FriendCount} friends for user {UserId}", friends.Count(), currentUser.Id);
                return View(friends);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching friends for user.");
                return RedirectToAction("Error", "Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> SearchUsers(string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    _logger.LogWarning("Search term is null or empty.");
                    return PartialView("_Error", new { message = "Search term cannot be empty." });
                }

                var users = await _userManager.Users
                    .Where(u => (u.UserName != null && u.UserName.Contains(searchTerm)) || (u.Email != null && u.Email.Contains(searchTerm)))
                    .ToListAsync();

                _logger.LogInformation("Fetched {UserCount} users for search term {SearchTerm}", users.Count, searchTerm);
                return PartialView("_UserSearchResults", users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while searching for users with term: {SearchTerm}", searchTerm);
                return PartialView("_Error", new { message = "An error occurred while searching for users." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddFriend(string friendId)
        {
            try
            {
                if (string.IsNullOrEmpty(friendId))
                {
                    _logger.LogWarning("Friend ID is null or empty.");
                    return Json(new { success = false, message = "Invalid friend ID." });
                }
                // Fetch the current user from the database
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser == null)
                {
                    _logger.LogWarning("Current user is null. Redirecting to login.");
                    return Json(new { success = false, message = "User not logged in." });
                }
                // Check for self-friendship
                if (currentUser.Id == friendId)
                {
                    _logger.LogWarning("User {UserId} tried to add themselves as a friend.", currentUser.Id);
                    return Json(new { success = false, message = "You cannot add yourself as a friend." });
                }
                // Check for existing friendship
                var existingFriendship = await _friendRepository.GetFriendshipAsync(currentUser.Id, friendId);
                if (existingFriendship != null)
                {
                    _logger.LogWarning("User {UserId} tried to add a friend {FriendId} they are already friends with.", currentUser.Id, friendId);
                    return Json(new { success = false, message = "You are already friends with this user." });
                }
                // Fetch the friend user from the database
                var friendUser = await _userManager.FindByIdAsync(friendId);
                if (friendUser == null)
                {
                    _logger.LogWarning("Friend user with ID {FriendId} not found.", friendId);
                    return Json(new { success = false, message = "Friend user not found." });
                }

                // Create the Friend object using the primary constructor
                var friendship = new Friend(currentUser.Id, friendId)
                {
                    User = currentUser,
                    FriendUser = friendUser
                };
                
                // Add the friendship to the repository
                await _friendRepository.AddFriendAsync(friendship);
                _logger.LogInformation("User {UserId} added friend {FriendId}", currentUser.Id, friendId);
                return Json(new { success = true, message = "Friend added successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding friend {FriendId} for user {UserId}", friendId, User?.Identity?.Name);
                return Json(new { success = false, message = "An error occurred while adding the friend." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteFriend(string friendId)
        {
            try
            {
                if (string.IsNullOrEmpty(friendId))
                {
                    _logger.LogWarning("Friend ID is null or empty.");
                    return Json(new { success = false, message = "Invalid friend ID." });
                }

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
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting friend {FriendId} for user {UserId}", friendId, User?.Identity?.Name);
                return Json(new { success = false, message = "An error occurred while deleting the friend." });
            }
        }
    }
}