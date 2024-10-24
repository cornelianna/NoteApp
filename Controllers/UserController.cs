using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NoteApp.Models; // Add this line to reference EditProfileViewModel

namespace NoteApp.Controllers
{
    [Authorize]
public class UserController : Controller
{
    public IActionResult Settings()
    {
        var model = new EditProfileViewModel(); // Provide an instance of the model
        // Optionally, populate the model with current user data

        return View(model); // This should map to Views/User/Settings.cshtml
    }
}

}
