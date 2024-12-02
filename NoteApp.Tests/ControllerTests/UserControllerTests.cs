// UserControllerTests.cs
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using NoteApp.Controllers;
using NoteApp.Repositories;
using Microsoft.AspNetCore.Mvc;
using NoteApp.Models;
using Microsoft.AspNetCore.Identity;

namespace NoteApp.Tests
{
    public class UserControllerTests
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IPostRepository> _mockPostRepository;
        private readonly Mock<ILogger<UserController>> _mockLogger;
        private readonly UserController _controller;

        public UserControllerTests()
        {
            // Initialize mocks for IUserRepository, IPostRepository, and ILogger<UserController>
            _mockUserRepository = new Mock<IUserRepository>();
            _mockPostRepository = new Mock<IPostRepository>();
            _mockLogger = new Mock<ILogger<UserController>>();

            // Instantiate the UserController with mocked dependencies
            _controller = new UserController(
                _mockUserRepository.Object,
                _mockPostRepository.Object,
                _mockLogger.Object);
        }

        // Positive test for Settings GET action
        [Fact]
        public void Settings_Get_ReturnsViewWithModel()
        {
            // Arrange
            // No additional setup is required since the Settings GET action does not depend on repositories

            // Act
            var result = _controller.Settings();

            // Assert
            // Verify that the result is a ViewResult
            var viewResult = Assert.IsType<ViewResult>(result);

            // Verify that the model passed to the view is of type EditProfileViewModel
            var model = Assert.IsAssignableFrom<EditProfileViewModel>(viewResult.ViewData.Model);

            // Verify that the logger was called with the correct parameters
            _mockLogger.Verify(
                logger => logger.Log(
                    It.Is<LogLevel>(logLevel => logLevel == LogLevel.Information),
                    It.IsAny<EventId>(),
                    It.IsAny<object>(),
                    It.IsAny<Exception?>(),
                    It.Is<Func<object, Exception?, string>>((state, exception) => true)),
                Times.Once);
        }

        // Negative test for Settings GET action
        [Fact]
        public void Settings_Get_WhenExceptionThrown_RedirectsToError()
        {
            // Arrange
            // Setup the logger to throw an exception when LogInformation is called
            _mockLogger
                .Setup(logger => logger.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception?>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()))
                .Throws(new Exception("Test exception"));

            // Act
            var result = _controller.Settings();

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Error", redirectResult.ActionName);
            Assert.Equal("Error", redirectResult.ControllerName);

            // Verify that LogError was called with the exception
            _mockLogger.Verify(
                logger => logger.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString() == "An error occurred while accessing the settings page."),
                    It.IsAny<Exception?>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        // Negative test for Settings POST action when model state is invalid
        [Fact]
        public async Task Settings_Post_WhenModelStateIsInvalid_ReturnsViewWithModel()
        {
            // Arrange
            var model = new EditProfileViewModel
            {
                Username = null, // Invalid because Username is required
                Email = "user@example.com",
                CurrentPassword = null,
                NewPassword = null
            };

            // Simulate an invalid model state
            _controller.ModelState.AddModelError("Username", "Username is required.");

            // Act
            var result = await _controller.Settings(model);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(model, viewResult.Model);

            // Verify that a warning was logged
            _mockLogger.Verify(
                logger => logger.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Invalid model state for settings update.")),
                    It.IsAny<Exception?>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        // Test for Settings POST when username update fails
        [Fact]
        public async Task Settings_Post_WhenSetUserNameFails_ReturnsViewWithModelErrors()
        {
            // Arrange
            var model = new EditProfileViewModel { Username = "NewUsername", Email = "user@example.com" };
            var user = new User { Id = "user-id", UserName = "OldUsername", Email = "user@example.com" };

            _mockUserRepository
                .Setup(repo => repo.GetUserAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .ReturnsAsync(user);

            _mockUserRepository
                .Setup(repo => repo.SetUserNameAsync(user, model.Username))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Username update failed." }));

            // Act
            var result = await _controller.Settings(model);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(model, viewResult.Model);
            Assert.True(_controller.ModelState.ContainsKey(string.Empty));
            Assert.Contains("Username update failed.", _controller.ModelState[string.Empty].Errors[0].ErrorMessage);

            // Verify a warning was logged
            _mockLogger.Verify(
                logger => logger.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Failed to update username for user user-id")),
                    It.IsAny<Exception?>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task Settings_Post_WhenEmailUpdateFails_ReturnsViewWithModelErrors()
        {
            // Arrange
            var model = new EditProfileViewModel
            {
                Username = "ExistingUser",
                Email = "newemail@example.com",
                CurrentPassword = null,
                NewPassword = null
            };

            var user = new User
            {
                Id = "user-id",
                UserName = "ExistingUser",
                Email = "oldemail@example.com"
            };

            _mockUserRepository
                .Setup(repo => repo.GetUserAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .ReturnsAsync(user);

            _mockUserRepository
                .Setup(repo => repo.SetEmailAsync(user, model.Email))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Email update failed." }));

            // Act
            var result = await _controller.Settings(model);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(model, viewResult.Model);
            Assert.True(_controller.ModelState.ContainsKey(string.Empty));
            Assert.Contains("Email update failed.", _controller.ModelState[string.Empty].Errors[0].ErrorMessage);

            // Verify warning log
            _mockLogger.Verify(
                logger => logger.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Failed to update email for user user-id")),
                    It.IsAny<Exception?>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task Settings_Post_WhenPasswordChangeFails_ReturnsViewWithModelErrors()
        {
            // Arrange
            var model = new EditProfileViewModel
            {
                Username = "ExistingUser",
                Email = "user@example.com",
                CurrentPassword = "oldpassword",
                NewPassword = "newpassword"
            };

            var user = new User
            {
                Id = "user-id",
                UserName = "ExistingUser",
                Email = "user@example.com"
            };

            _mockUserRepository
                .Setup(repo => repo.GetUserAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .ReturnsAsync(user);

            _mockUserRepository
                .Setup(repo => repo.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Password change failed." }));

            // Act
            var result = await _controller.Settings(model);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(model, viewResult.Model);
            Assert.True(_controller.ModelState.ContainsKey(string.Empty));
            Assert.Contains("Password change failed.", _controller.ModelState[string.Empty].Errors[0].ErrorMessage);

            // Verify warning log
            _mockLogger.Verify(
                logger => logger.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Failed to change password for user user-id")),
                    It.IsAny<Exception?>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task Profile_WhenUserFoundWithPosts_ReturnsViewWithModel()
        {
            // Arrange
            string userId = "test-user-id";
            var user = new User
            {
                Id = userId,
                UserName = "TestUser"
            };

            var posts = new List<Post>
            {
                new Post { Id = 1, Content = "First post" },
                new Post { Id = 2, Content = "Second post" }
            };

            _mockUserRepository
                .Setup(repo => repo.FindByIdAsync(userId))
                .ReturnsAsync(user);

            _mockPostRepository
                .Setup(repo => repo.GetPostsByUserIdAsync(userId))
                .ReturnsAsync(posts);

            // Act
            var result = await _controller.Profile(userId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<UserProfileViewModel>(viewResult.Model);

            Assert.Equal(user.Id, model.UserId);
            Assert.Equal(user.UserName, model.Username);
            Assert.Equal(posts.Count, model.Posts.Count);
            Assert.Equal(posts, model.Posts);

            // Verify logs
            _mockLogger.Verify(
                logger => logger.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"Fetched {posts.Count} posts for user {userId}")),
                    It.IsAny<Exception?>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);

            _mockLogger.Verify(
                logger => logger.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"Displaying profile for user {userId}")),
                    It.IsAny<Exception?>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task Profile_WhenUserNotFound_ReturnsNotFound()
        {
            // Arrange
            string userId = "nonexistent-user";

            _mockUserRepository
                .Setup(repo => repo.FindByIdAsync(userId))
                .ReturnsAsync((User)null);

            // Act
            var result = await _controller.Profile(userId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("User not found.", notFoundResult.Value);

            // Verify warning log
            _mockLogger.Verify(
                logger => logger.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"User with ID {userId} not found.")),
                    It.IsAny<Exception?>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

    }
}
