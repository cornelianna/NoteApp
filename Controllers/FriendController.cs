using Microsoft.AspNetCore.Mvc;
using NoteApp.Repositories;
using Microsoft.AspNetCore.Identity;

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
                // Optionally redirect to login
                return RedirectToAction("Login", "Account");
            }

            var friends = await _friendRepository.GetFriendsByUserIdAsync(currentUser.Id);
            return View(friends);
        }
    }
}
