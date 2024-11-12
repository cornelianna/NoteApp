using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NoteApp.Controllers;
using NoteApp.Models;
using NoteApp.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Extensions.Logging;

namespace NoteApp.Test.Controllers
{
    public class FriendControllerTests
    {
        private readonly Mock<IFriendRepository> _mockFriendRepository;
        private readonly Mock<UserManager<IdentityUser>> _mockUserManager;
        private readonly Mock<ILogger<FriendController>> _mockLogger;
        private readonly FriendController _friendController;

        public FriendControllerTests()
        {
            _mockFriendRepository = new Mock<IFriendRepository>();
            _mockUserManager = new Mock<UserManager<IdentityUser>>(
                new Mock<IUserStore<IdentityUser>>().Object, null, null, null, null, null, null, null, null);
            _mockLogger = new Mock<ILogger<FriendController>>();
            _friendController = new FriendController(_mockFriendRepository.Object, _mockUserManager.Object, _mockLogger.Object);
        }

        // Positive test for index

        [Fact]
        public async Task Index_AuthenticatedUser_ReturnsViewWithFriends()
        {
            // Arrange
            var user = new IdentityUser { Id = "test-user-id" };
            var friends = new List<Friend> { new Friend { UserId = "test-user-id", FriendId = "friend-1" } };

            _mockUserManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            _mockFriendRepository.Setup(repo => repo.GetFriendsByUserIdAsync(user.Id)).ReturnsAsync(friends);

            // Act
            var result = await _friendController.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(friends, viewResult.Model);
            _mockLogger.Verify(logger => logger.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)), Times.Once);
        }

        // Negative test for index
        [Fact]
        public async Task Index_UnauthenticatedUser_RedirectsToLogin()
        {
            // Arrange
            _mockUserManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync((IdentityUser)null);

            // Act
            var result = await _friendController.Index();

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Login", redirectResult.ActionName);
            Assert.Equal("Account", redirectResult.ControllerName);

            _mockLogger.Verify(logger => logger.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception?>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)), Times.Once);
        }

        // Positive test for search users
        // får enten denne feilen:
        // System.ArgumentNullException : Value cannot be null. (Parameter 'logger')
        // eller denne feilen:
        //  Assert.Equal() Failure: Strings differ
        //     ↓ (pos 1)
        // Expected: "_UserSearchResults"
        //Actual:   "_Error"
        //    ↑ (pos 1)

        // positive test for add friend
        // fungerer heller ikke

        // Negative test for search users
        // fungerer heller ikke

        


        

       










    }

}