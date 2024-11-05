// AccountController.cs
using Microsoft.AspNetCore.Mvc;
using NoteApp.Models;
using NoteApp.Repositories;
using System.Threading.Tasks;

namespace NoteApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ILogger<AccountController> _logger;

        public AccountController(IAccountRepository accountRepository, ILogger<AccountController> logger)
        {
            _accountRepository = accountRepository;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await _accountRepository.RegisterAsync(model);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index", "Post");
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during registration.");
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await _accountRepository.LoginAsync(model);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index", "Post");
                    }
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                }
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during login.");
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await _accountRepository.LogoutAsync();
                return RedirectToAction("Index", "Post");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during logout.");
                return RedirectToAction("Error", "Home");
            }
        }
    }
}