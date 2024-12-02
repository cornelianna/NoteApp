// PostControllerTests.cs
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using NoteApp.Controllers;
using NoteApp.Repositories;
using Microsoft.AspNetCore.Mvc;
using NoteApp.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System;

namespace NoteApp.Tests
{
    public class PostControllerTests
    {
        private readonly Mock<IPostRepository> _mockPostRepository;
        private readonly Mock<ICommentRepository> _mockCommentRepository;
        private readonly Mock<ILogger<PostController>> _mockLogger;
        private readonly PostController _controller;

        public PostControllerTests()
        {
            _mockPostRepository = new Mock<IPostRepository>();
            _mockCommentRepository = new Mock<ICommentRepository>();
            _mockLogger = new Mock<ILogger<PostController>>();

            _controller = new PostController(
                _mockPostRepository.Object,
                _mockCommentRepository.Object,
                _mockLogger.Object);
        }

        // Positive test for Index action
        [Fact]
        public async Task Index_ReturnsViewWithPosts()
        {
            // Arrange
            var posts = new List<Post>
            {
                new Post { Id = 1, Content = "Post 1" },
                new Post { Id = 2, Content = "Post 2" }
            };

            _mockPostRepository.Setup(repo => repo.GetAllPostsAsync())
                .ReturnsAsync(posts);

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(posts, viewResult.Model);

            // Verify that LogInformation was called
            _mockLogger.Verify(logger => logger.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true), // Ignore message content
                It.IsAny<Exception?>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)), Times.Once);
        }

        // Negative test for Index action when an exception is thrown
        [Fact]
        public async Task Index_ExceptionThrown_ReturnsRedirectToError()
        {
            // Arrange
            _mockPostRepository.Setup(repo => repo.GetAllPostsAsync())
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.Index();

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Error", redirectResult.ActionName);

            // Verify that LogError was called
            _mockLogger.Verify(logger => logger.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true), // Ignore message content
                It.IsAny<Exception?>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)), Times.Once);
        }

        // Positive test for CreatePost action
        [Fact]
        public async Task CreatePost_ValidPost_RedirectsToIndex()
        {
            // Arrange
            var userId = "user123";
            var userName = "testuser";

            // Set up the User in ControllerContext
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Name, userName)
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            var post = new Post { Content = "Test content" };
            IFormFile image = null; // No image in this test

            // Act
            var result = await _controller.CreatePost(post, image);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);

            // Verify that AddPostAsync was called
            _mockPostRepository.Verify(repo => repo.AddPostAsync(It.Is<Post>(p => p.Content == post.Content && p.UserId == userId)), Times.Once);

            // Verify that LogInformation was called
            _mockLogger.Verify(logger => logger.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true), // Ignore message content
                It.IsAny<Exception?>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)), Times.Once);
        }

        // Negative test for CreatePost action when post is null
        [Fact]
        public async Task CreatePost_NullPost_ReturnsBadRequest()
        {
            // Arrange
            Post post = null;
            IFormFile image = null;

            // Act
            var result = await _controller.CreatePost(post, image);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid post object.", badRequestResult.Value);

            // Verify that LogWarning was called
            _mockLogger.Verify(logger => logger.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true), // Ignore message content
                It.IsAny<Exception?>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)), Times.Once);
        }

        // Positive test for DeletePost action
        [Fact]
        public async Task DeletePost_UserAuthorized_DeletesPostAndRedirectsToIndex()
        {
            // Arrange
            var userId = "user123";
            var postId = 1;
            var post = new Post { Id = postId, UserId = userId };

            // Set up User in ControllerContext
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId)
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            // Mock GetPostByIdAsync to return the post
            _mockPostRepository.Setup(repo => repo.GetPostByIdAsync(postId))
                .ReturnsAsync(post);

            // Act
            var result = await _controller.DeletePost(postId);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);

            // Verify that DeletePostAsync was called
            _mockPostRepository.Verify(repo => repo.DeletePostAsync(postId), Times.Once);

            // Verify that LogInformation was called
            _mockLogger.Verify(logger => logger.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true), // Ignore message content
                It.IsAny<Exception?>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)), Times.Once);
        }

        // Negative test for DeletePost action when user is not authorized
        [Fact]
        public async Task DeletePost_UserNotAuthorized_ReturnsForbid()
        {
            // Arrange
            var userId = "user123";
            var otherUserId = "user456";
            var postId = 1;
            var post = new Post { Id = postId, UserId = otherUserId };

            // Set up User in ControllerContext
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId)
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            // Mock GetPostByIdAsync to return the post
            _mockPostRepository.Setup(repo => repo.GetPostByIdAsync(postId))
                .ReturnsAsync(post);

            // Act
            var result = await _controller.DeletePost(postId);

            // Assert
            Assert.IsType<ForbidResult>(result);

            // Verify that DeletePostAsync was not called
            _mockPostRepository.Verify(repo => repo.DeletePostAsync(It.IsAny<int>()), Times.Never);

            // Verify that LogWarning was called
            _mockLogger.Verify(logger => logger.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true), // Ignore message content
                It.IsAny<Exception?>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)), Times.Once);
        }

        // Positive test for UpdatePost GET action
        [Fact]
        public async Task UpdatePost_Get_UserAuthorized_ReturnsViewWithPost()
        {
            // Arrange
            var userId = "user123";
            var postId = 1;
            var post = new Post { Id = postId, UserId = userId };

            // Set up User in ControllerContext
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId)
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            _mockPostRepository.Setup(repo => repo.GetPostByIdAsync(postId))
                .ReturnsAsync(post);

            // Act
            var result = await _controller.UpdatePost(postId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(post, viewResult.Model);
        }

        // Negative test for UpdatePost GET action when user is not authorized
        [Fact]
        public async Task UpdatePost_Get_UserNotAuthorized_ReturnsForbid()
        {
            // Arrange
            var userId = "user123";
            var otherUserId = "user456";
            var postId = 1;
            var post = new Post { Id = postId, UserId = otherUserId };

            // Set up User in ControllerContext
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId)
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            _mockPostRepository.Setup(repo => repo.GetPostByIdAsync(postId))
                .ReturnsAsync(post);

            // Act
            var result = await _controller.UpdatePost(postId);

            // Assert
            Assert.IsType<ForbidResult>(result);

            // Verify that LogWarning was called
            _mockLogger.Verify(logger => logger.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true), // Ignore message content
                It.IsAny<Exception?>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)), Times.Once);
        }

        // Positive test for UpdatePost POST action
        [Fact]
        public async Task UpdatePost_Post_UserAuthorized_UpdatesPostAndRedirectsToIndex()
        {
            // Arrange
            var userId = "user123";
            var postId = 1;
            var post = new Post { Id = postId, UserId = userId, Content = "Old content" };
            var updatedPost = new Post { Content = "Updated content" };

            // Set up User in ControllerContext
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId)
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            _mockPostRepository.Setup(repo => repo.GetPostByIdAsync(postId))
                .ReturnsAsync(post);

            // Act
            var result = await _controller.UpdatePost(postId, updatedPost, null);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);

            // Verify that UpdatePostAsync was called
            _mockPostRepository.Verify(repo => repo.UpdatePostAsync(It.Is<Post>(p => p.Id == postId && p.Content == updatedPost.Content)), Times.Once);

            // Verify that LogInformation was called
            _mockLogger.Verify(logger => logger.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true), // Ignore message content
                It.IsAny<Exception?>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)), Times.Once);
        }

        // Negative test for UpdatePost POST action when user is not authorized
        [Fact]
        public async Task UpdatePost_Post_UserNotAuthorized_ReturnsForbid()
        {
            // Arrange
            var userId = "user123";
            var otherUserId = "user456";
            var postId = 1;
            var post = new Post { Id = postId, UserId = otherUserId, Content = "Old content" };
            var updatedPost = new Post { Content = "Updated content" };

            // Set up User in ControllerContext
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId)
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            _mockPostRepository.Setup(repo => repo.GetPostByIdAsync(postId))
                .ReturnsAsync(post);

            // Act
            var result = await _controller.UpdatePost(postId, updatedPost, null);

            // Assert
            Assert.IsType<ForbidResult>(result);

            // Verify that UpdatePostAsync was not called
            _mockPostRepository.Verify(repo => repo.UpdatePostAsync(It.IsAny<Post>()), Times.Never);

            // Verify that LogWarning was called
            _mockLogger.Verify(logger => logger.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true), // Ignore message content
                It.IsAny<Exception?>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)), Times.Once);
        }

        // Positive test for AddComment action
        [Fact]
        public async Task AddComment_ValidComment_RedirectsToViewPost()
        {
            // Arrange
            var userId = "user123";
            var userName = "testuser";
            var postId = 1;
            var commentContent = "Test comment";
            var returnUrl = "ViewPost";

            // Create the comment to be added
            var comment = new Comment { Content = commentContent, PostId = postId };

            // Set up the User in ControllerContext (simulate logged-in user)
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Name, userName)
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            // Mock GetPostByIdAsync to return a valid post
            var post = new Post { Id = postId, Content = "Test post", UserId = "user456" };
            _mockPostRepository.Setup(repo => repo.GetPostByIdAsync(postId))
                .ReturnsAsync(post);

            // Act
            var result = await _controller.AddComment(comment, returnUrl);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ViewPost", redirectResult.ActionName);
            Assert.Equal(postId, redirectResult.RouteValues["id"]);

            // Verify that AddCommentAsync was called with the correct comment
            _mockCommentRepository.Verify(repo => repo.AddCommentAsync(It.Is<Comment>(c =>
                c.Content == commentContent &&
                c.UserId == userId &&
                c.Username == userName &&
                c.PostId == postId &&
                c.Post == post)), Times.Once);

            // Verify that LogInformation was called
            _mockLogger.Verify(logger => logger.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<object>((v, t) => true), // Ignore message content
                It.IsAny<Exception?>(),
                It.Is<Func<object, Exception?, string>>((v, t) => true)), Times.Once);
        }



        // Negative test for AddComment action when comment is null
        [Fact]
        public async Task AddComment_NullComment_ReturnsBadRequest()
        {
            // Arrange
            Comment comment = null;
            string returnUrl = null;

            // Act
            var result = await _controller.AddComment(comment, returnUrl);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid comment.", badRequestResult.Value);

            // Verify that LogWarning was called
            _mockLogger.Verify(logger => logger.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true), // Ignore message content
                It.IsAny<Exception?>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)), Times.Once);
        }

        // Positive test for DeleteComment action
        [Fact]
        public async Task DeleteComment_UserAuthorized_DeletesCommentAndRedirectsToIndex()
        {
            // Arrange
            var userId = "user123";
            var commentId = 1;
            var comment = new Comment { Id = commentId, UserId = userId };

            // Set up User in ControllerContext
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId)
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            _mockCommentRepository.Setup(repo => repo.GetCommentByIdAsync(commentId))
                .ReturnsAsync(comment);

            // Act
            var result = await _controller.DeleteComment(commentId);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);

            // Verify that DeleteCommentAsync was called
            _mockCommentRepository.Verify(repo => repo.DeleteCommentAsync(commentId), Times.Once);

            // Verify that LogInformation was called
            _mockLogger.Verify(logger => logger.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true), // Ignore message content
                It.IsAny<Exception?>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)), Times.Once);
        }

        // Negative test for DeleteComment action when user is not authorized
        [Fact]
        public async Task DeleteComment_UserNotAuthorized_ReturnsForbid()
        {
            // Arrange
            var userId = "user123";
            var otherUserId = "user456";
            var commentId = 1;
            var comment = new Comment { Id = commentId, UserId = otherUserId };

            // Set up User in ControllerContext
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId)
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            _mockCommentRepository.Setup(repo => repo.GetCommentByIdAsync(commentId))
                .ReturnsAsync(comment);

            // Act
            var result = await _controller.DeleteComment(commentId);

            // Assert
            Assert.IsType<ForbidResult>(result);

            // Verify that DeleteCommentAsync was not called
            _mockCommentRepository.Verify(repo => repo.DeleteCommentAsync(It.IsAny<int>()), Times.Never);

            // Verify that LogWarning was called
            _mockLogger.Verify(logger => logger.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true), // Ignore message content
                It.IsAny<Exception?>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)), Times.Once);
        }

        // Positive test for UpdateComment GET action
        [Fact]
        public async Task UpdateComment_Get_UserAuthorized_ReturnsViewWithComment()
        {
            // Arrange
            var userId = "user123";
            var commentId = 1;
            var comment = new Comment { Id = commentId, UserId = userId };

            // Set up User in ControllerContext
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId)
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            _mockCommentRepository.Setup(repo => repo.GetCommentByIdAsync(commentId))
                .ReturnsAsync(comment);

            // Act
            var result = await _controller.UpdateComment(commentId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(comment, viewResult.Model);
        }

        // Negative test for UpdateComment GET action when user is not authorized
        [Fact]
        public async Task UpdateComment_Get_UserNotAuthorized_ReturnsForbid()
        {
            // Arrange
            var userId = "user123";
            var otherUserId = "user456";
            var commentId = 1;
            var comment = new Comment { Id = commentId, UserId = otherUserId };

            // Set up User in ControllerContext
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId)
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            _mockCommentRepository.Setup(repo => repo.GetCommentByIdAsync(commentId))
                .ReturnsAsync(comment);

            // Act
            var result = await _controller.UpdateComment(commentId);

            // Assert
            Assert.IsType<ForbidResult>(result);

            // Verify that LogWarning was called
            _mockLogger.Verify(logger => logger.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true), // Ignore message content
                It.IsAny<Exception?>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)), Times.Once);
        }
    }
}
