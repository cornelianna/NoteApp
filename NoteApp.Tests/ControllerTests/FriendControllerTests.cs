using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using NoteApp.Controllers;
using NoteApp.Models;
using NoteApp.Repositories;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace NoteApp.Tests.ControllersTests
{
    public class FriendControllerTests
    {
        private readonly FriendController _controller;
        private readonly Mock<UserManager<IdentityUser>> _mockUserManager;
        private readonly Mock<IFriendRepository> _mockFriendRepository;
        private readonly Mock<ILogger<FriendController>> _mockLogger;

        public FriendControllerTests()
        {
            _mockFriendRepository = new Mock<IFriendRepository>();
            _mockUserManager = new Mock<UserManager<IdentityUser>>(
                new Mock<IUserStore<IdentityUser>>().Object, null, null, null, null, null, null, null, null);
            _mockLogger = new Mock<ILogger<FriendController>>();

            _controller = new FriendController(_mockFriendRepository.Object, _mockUserManager.Object, _mockLogger.Object);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "user123")
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };
        }

        [Fact]
        public async Task Index_ReturnsViewWithFriends()
        {
            // Arrange
            var userId = "user123";
            var friends = new List<Friend>
            {
                new Friend { UserId = userId, FriendId = "friend1" },
                new Friend { UserId = userId, FriendId = "friend2" }
            };

            _mockFriendRepository.Setup(repo => repo.GetFriendsByUserIdAsync(userId))
                .ReturnsAsync(friends);

            _mockUserManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(new IdentityUser { Id = userId });

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(friends, viewResult.Model);
        }

        // Negative test for Index user not found
        [Fact]
        public async Task Index_UserNotFound_ReturnsRedirectToActionResult()
        {
            // Arrange
            var userId = "user123";

            _mockUserManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync((IdentityUser)null); // Simulate user not found

            // Act
            var result = await _controller.Index();

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Login", redirectResult.ActionName);
            Assert.Equal("Account", redirectResult.ControllerName);
        }

        [Fact]
        public async Task SearchUsers_ValidSearchTerm_ReturnsPartialViewWithUsers()
        {
            // Arrange
            var searchTerm = "test";
            var users = new List<IdentityUser>
            {
                new IdentityUser { UserName = "testuser1" },
                new IdentityUser { UserName = "testuser2" }
            };

            _mockUserManager.Setup(um => um.Users)
                .Returns(users.AsQueryable());

            // Act
            var result = await _controller.SearchUsers(searchTerm);

            // Assert
            var partialViewResult = Assert.IsType<PartialViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<IdentityUser>>(partialViewResult.Model);
            Assert.NotEmpty(model);
            Assert.All(model, user => Assert.Contains(searchTerm, user.UserName));
        }
    }
}