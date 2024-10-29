<<<<<<< HEAD
using Microsoft.AspNetCore.Mvc;
using NoteApp.Repositories;
using Microsoft.AspNetCore.Identity;
using Serilog;
using Microsoft.EntityFrameworkCore;
using NoteApp.Models;
=======
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using NoteApp.Models;
using NoteApp.Repositories;
>>>>>>> origin/anna-test

namespace NoteApp.Controllers
{
    public class FriendController : Controller
    {
        private readonly IFriendRepository _friendRepository;
        private readonly UserManager<IdentityUser> _userManager;
<<<<<<< HEAD
        private readonly ILogger<FriendController> _logger;

        public FriendController(IFriendRepository friendRepository, UserManager<IdentityUser> userManager, ILogger<FriendController> logger)
        {
            _friendRepository = friendRepository;
            _userManager = userManager;
            _logger = logger;
=======

        public FriendController(IFriendRepository friendRepository, UserManager<IdentityUser> userManager)
        {
            _friendRepository = friendRepository;
            _userManager = userManager;
>>>>>>> origin/anna-test
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
<<<<<<< HEAD
                _logger.LogWarning("Current user is null. Redirecting to login.");
=======
                // Redirect to login if user is not authenticated
>>>>>>> origin/anna-test
                return RedirectToAction("Login", "Account");
            }

            var friends = await _friendRepository.GetFriendsByUserIdAsync(currentUser.Id);
<<<<<<< HEAD
            _logger.LogInformation("Fetched friends for user {UserId}", currentUser.Id);
            return View(friends);
        }

        [HttpGet]
        public async Task<IActionResult> SearchUsers(string query)
        {
            var users = await _userManager.Users
                .Where(u => u.UserName.Contains(query) || u.Email.Contains(query))
                .ToListAsync();

            return PartialView("_UserSearchResultsPartial", users);
        }

=======
            return View(friends);
        }

        // Add this action to search for users
        [HttpPost]
        public async Task<IActionResult> SearchUsers(string searchTerm)
        {
            var users = await _userManager.Users
                .Where(u => u.UserName.Contains(searchTerm) || u.Email.Contains(searchTerm))
                .ToListAsync();

            return PartialView("_UserSearchResults", users);
        }

        // Add this action to add a friend
>>>>>>> origin/anna-test
        [HttpPost]
        public async Task<IActionResult> AddFriend(string friendId)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
<<<<<<< HEAD
                _logger.LogWarning("Current user is null. Redirecting to login.");
                return RedirectToAction("Login", "Account");
            }

            var friend = new Friend
=======
                return Json(new { success = false, message = "User not logged in." });
            }

            if (currentUser.Id == friendId)
            {
                return Json(new { success = false, message = "You cannot add yourself as a friend." });
            }

            var existingFriendship = await _friendRepository.GetFriendshipAsync(currentUser.Id, friendId);
            if (existingFriendship != null)
            {
                return Json(new { success = false, message = "You are already friends with this user." });
            }

            var friendship = new Friend
>>>>>>> origin/anna-test
            {
                UserId = currentUser.Id,
                FriendId = friendId
            };

<<<<<<< HEAD
            await _friendRepository.AddFriendAsync(friend);
            _logger.LogInformation("User {UserId} added friend {FriendId}", currentUser.Id, friendId);

            return RedirectToAction("Index");
        }
    }
}
=======
            await _friendRepository.AddFriendAsync(friendship);
            return Json(new { success = true, message = "Friend added successfully." });
        }
        [HttpPost]
        public async Task<IActionResult> DeleteFriend(string friendId)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Json(new { success = false, message = "User not logged in." });
            }

            await _friendRepository.DeleteFriendAsync(currentUser.Id, friendId);
            return Json(new { success = true, message = "Friend deleted successfully." });
        }

    }
}
>>>>>>> origin/anna-test
