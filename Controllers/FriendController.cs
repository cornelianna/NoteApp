using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using NoteApp.Models;
using NoteApp.Repositories;

namespace NoteApp.Controllers
{
    public class FriendController : Controller
    {
        private readonly IFriendRepository _friendRepository;
        private readonly UserManager<IdentityUser> _userManager;

        public FriendController(IFriendRepository friendRepository, UserManager<IdentityUser> userManager)
        {
            _friendRepository = friendRepository;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                // Redirect to login if user is not authenticated
                return RedirectToAction("Login", "Account");
            }

            var friends = await _friendRepository.GetFriendsByUserIdAsync(currentUser.Id);
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
        [HttpPost]
        public async Task<IActionResult> AddFriend(string friendId)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
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
            {
                UserId = currentUser.Id,
                FriendId = friendId
            };

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
