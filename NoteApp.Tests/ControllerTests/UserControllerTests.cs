using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NoteApp.Controllers;
using NoteApp.Models;
using NoteApp.Repositories;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace NoteApp.Test.Controllers
{
    public class UserControllerTests
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IPostRepository> _mockPostRepository;
        private readonly Mock<ILogger<UserController>> _mockLogger;
        private readonly UserController _userController;

        public UserControllerTests()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _mockPostRepository = new Mock<IPostRepository>();
            _mockLogger = new Mock<ILogger<UserController>>();

            _userController = new UserController(_mockUserRepository.Object, _mockPostRepository.Object, _mockLogger.Object);

            // Setup mock user identity for tests that require authentication
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "test-user-id"),
                new Claim(ClaimTypes.Name, "testuser")
            }, "mock"));
            _userController.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = user } };
        }

        // GET test for settings

        [Fact]
        public void Settings_Get_ReturnsViewWithModel()
        {
            // Arrange
            var mockUserRepository = new Mock<IUserRepository>();
            var mockPostRepository = new Mock<IPostRepository>();
            var mockLogger = new Mock<ILogger<UserController>>();
            var userController = new UserController(mockUserRepository.Object, mockPostRepository.Object, mockLogger.Object);

            // Act
            var result = userController.Settings();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsType<EditProfileViewModel>(viewResult.Model);
        }

        // POST test for settings

        [Fact]
        public async Task Settings_Post_ValidModel_ReturnsRedirectToIndex()
        {
            // Arrange
            var model = new EditProfileViewModel { Username = "newusername", Email = "newemail@example.com" };
            var user = new IdentityUser { Id = "user-id", UserName = "oldusername", Email = "oldemail@example.com" };

            var mockUserRepository = new Mock<IUserRepository>();
            mockUserRepository.Setup(repo => repo.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            mockUserRepository.Setup(repo => repo.SetUserNameAsync(user, model.Username)).ReturnsAsync(IdentityResult.Success);
            mockUserRepository.Setup(repo => repo.SetEmailAsync(user, model.Email)).ReturnsAsync(IdentityResult.Success);
            mockUserRepository.Setup(repo => repo.RefreshSignInAsync(user)).Returns(Task.CompletedTask);

            var mockLogger = new Mock<ILogger<UserController>>();
            var userController = new UserController(mockUserRepository.Object, new Mock<IPostRepository>().Object, mockLogger.Object);

            // Act
            var result = await userController.Settings(model);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }

        // Test for user profile

        [Fact]
        public async Task Profile_WithCurrentUser_ReturnsUserProfile()
        {
            // Arrange
            var userId = "user-id";
            var user = new IdentityUser { Id = userId, UserName = "username" };
            var posts = new List<Post> { new Post { Id = 1, Content = "Test post", UserId = userId } };

            var mockUserRepository = new Mock<IUserRepository>();
            mockUserRepository.Setup(repo => repo.GetUserIdAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(userId);
            mockUserRepository.Setup(repo => repo.FindByIdAsync(userId)).ReturnsAsync(user);

            var mockPostRepository = new Mock<IPostRepository>();
            mockPostRepository.Setup(repo => repo.GetPostsByUserIdAsync(userId)).ReturnsAsync(posts);

            var userController = new UserController(mockUserRepository.Object, mockPostRepository.Object, new Mock<ILogger<UserController>>().Object);

            // Act
            var result = await userController.Profile(null);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<UserProfileViewModel>(viewResult.Model);
            Assert.Equal(userId, model.UserId);
            Assert.Equal("username", model.Username);
            Assert.Single(model.Posts);
        }











    }

}