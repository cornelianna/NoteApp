using Microsoft.AspNetCore.Mvc;
using NoteApp.Repositories;
using Microsoft.AspNetCore.Identity;
using Serilog;
using Microsoft.EntityFrameworkCore;
using NoteApp.Models;

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
                return RedirectToAction("Login", "Account");
            }

            var friends = await _friendRepository.GetFriendsByUserIdAsync(currentUser.Id);
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

        [HttpPost]
        public async Task<IActionResult> AddFriend(string friendId)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                _logger.LogWarning("Current user is null. Redirecting to login.");
                return RedirectToAction("Login", "Account");
            }

            var friend = new Friend
            {
                UserId = currentUser.Id,
                FriendId = friendId
            };

            await _friendRepository.AddFriendAsync(friend);
            _logger.LogInformation("User {UserId} added friend {FriendId}", currentUser.Id, friendId);

            return RedirectToAction("Index");
        }
    }
}