using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NoteApp.Data;
using NoteApp.Models;
using System.Linq;
using System.Threading.Tasks;

namespace NoteApp.Controllers
{
    [Authorize]
    public class FriendsController : Controller
    {
        private readonly NoteAppContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<FriendsController> _logger;

        public FriendsController(NoteAppContext context, UserManager<IdentityUser> userManager, ILogger<FriendsController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        public IActionResult Index(string searchQuery)
        {
            var currentUserId = _userManager.GetUserId(User);
            var friends = _context.Friendships
                .Where(f => f.UserId == currentUserId)
                .Select(f => f.Friend)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                friends = friends.Where(u => (u.UserName != null && u.UserName.Contains(searchQuery)) || (u.Email != null && u.Email.Contains(searchQuery)));
            }

            return View(friends.ToList());
        }

        [HttpPost]
        public IActionResult Search(string query)
        {
            return RedirectToAction("Index", new { searchQuery = query });
        }

        [HttpGet]
        public IActionResult SearchUsers(string query)
        {
            var users = _context.Users
                .Where(u => (u.UserName != null && u.UserName.Contains(query)) || (u.Email != null && u.Email.Contains(query)))
                .Select(u => new { u.Id, u.UserName, u.Email })
                .ToList();

            return Json(users);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddFriend([FromBody] AddFriendRequest request)
        {
            try
            {
                var currentUser = await _userManager.GetUserAsync(User);
                var friend = await _userManager.FindByIdAsync(request.FriendId);

                if (currentUser != null && friend != null)
                {
                    var friendship = new Friendship
                    {
                        UserId = currentUser.Id,
                        FriendId = friend.Id,
                        User = currentUser,
                        Friend = friend
                    };

                    _context.Friendships.Add(friendship);
                    await _context.SaveChangesAsync();

                    return Ok(new { success = true, message = "Friend added successfully.", friend = new { friend.Id, friend.UserName, friend.Email } });
                }

                return BadRequest(new { success = false, message = "Failed to add friend." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding a friend.");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteFriend([FromBody] DeleteFriendRequest request)
        {
            try
            {
                var currentUser = await _userManager.GetUserAsync(User);
                var friend = await _userManager.FindByIdAsync(request.FriendId);

                if (currentUser != null && friend != null)
                {
                    var friendship = _context.Friendships
                        .FirstOrDefault(f => f.UserId == currentUser.Id && f.FriendId == friend.Id);

                    if (friendship != null)
                    {
                        _context.Friendships.Remove(friendship);
                        await _context.SaveChangesAsync();
                    }

                    return Ok(new { success = true, message = "Friend deleted successfully." });
                }

                return BadRequest(new { success = false, message = "Failed to delete friend." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting a friend.");
                return StatusCode(500, "Internal server error");
            }
        }
    }

    public class AddFriendRequest
    {
        public required string FriendId { get; set; }
    }

    public class DeleteFriendRequest
    {
        public required string FriendId { get; set; }
    }
}