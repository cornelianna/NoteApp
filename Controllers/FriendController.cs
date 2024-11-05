using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using NoteApp.Models;
using Serilog;
using NoteApp.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace NoteApp.Controllers
{
    [Authorize]
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
            try
            {
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser == null)
                {
                    _logger.LogWarning("User is not authenticated. Redirecting to login page.");
                    return RedirectToAction("Login", "Account");
                }

                var friends = await _friendRepository.GetFriendsByUserIdAsync(currentUser.Id);
                _logger.LogInformation("Fetched friends for user {UserId}", currentUser.Id);
                return View(friends);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching friends.");
                // Redirect to the Error action in PostController
                return RedirectToAction("Error", "Post");
            }
        }

        [HttpPost]
        public async Task<IActionResult> SearchUsers(string searchTerm)
        {
            try
            {
                var users = await _userManager.Users
                    .Where(u => u.UserName.Contains(searchTerm) || u.Email.Contains(searchTerm))
                    .ToListAsync();

                _logger.LogInformation("Fetched users for search term {SearchTerm}", searchTerm);
                return PartialView("_UserSearchResults", users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while searching for users.");
                return PartialView("_Error", new { message = "An error occurred while searching for users." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddFriend(string friendId)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding a friend.");
                return Json(new { success = false, message = "An error occurred while adding the friend." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteFriend(string friendId)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting a friend.");
                return Json(new { success = false, message = "An error occurred while deleting the friend." });
            }
        }
    }
}
