using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NoteApp.Controllers;
using NoteApp.Models;
using NoteApp.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace NoteApp.Test.FriendControllerTests
{

    public class PostControllerTests
    {
        private readonly Mock<IPostRepository> _mockPostRepository;
        private readonly Mock<ICommentRepository> _mockCommentRepository;
        private readonly Mock<ILogger<PostController>> _mockLogger;
        private readonly PostController _postController;

        public PostControllerTests()
        {
            _mockPostRepository = new Mock<IPostRepository>();
            _mockCommentRepository = new Mock<ICommentRepository>();
            _mockLogger = new Mock<ILogger<PostController>>();

            _postController = new PostController(_mockPostRepository.Object, _mockCommentRepository.Object, _mockLogger.Object);

            // Setup mock user identity for tests that require authentication
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "test-user-id"),
                new Claim(ClaimTypes.Name, "testuser")
            }, "mock"));
            _postController.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = user } };
        }

        // Test for index

        [Fact]
        public async Task Index_ReturnsViewWithPosts()
        {
            // Arrange
            var mockPosts = new List<Post>
            {
                new Post { Id = 1, Content = "Post 1", UserId = "user1" },
                new Post { Id = 2, Content = "Post 2", UserId = "user2" }
            };
            _mockPostRepository.Setup(repo => repo.GetAllPostsAsync()).ReturnsAsync(mockPosts);

            // Act
            var result = await _postController.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Post>>(viewResult.Model);
            Assert.Equal(2, model.Count());
        }

        // Positive test for create post

        [Fact]
        public async Task CreatePost_WithValidPostAndImage_ReturnsRedirectToIndex()
        {
            // Arrange
            var post = new Post { Content = "New Post" };
            var image = new FormFile(new MemoryStream(new byte[256]), 0, 256, "Data", "image.jpg");

            _mockPostRepository.Setup(repo => repo.AddPostAsync(It.IsAny<Post>())).Returns(Task.CompletedTask);

            // Act
            var result = await _postController.CreatePost(post, image);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            _mockPostRepository.Verify(repo => repo.AddPostAsync(It.IsAny<Post>()), Times.Once);
        }

        // Negative test for create post

        [Fact]
        public async Task CreatePost_WithNullPost_ReturnsBadRequest()
        {
            // Arrange
            Post post = null;
            FormFile image = null;

            // Act
            var result = await _postController.CreatePost(post, image);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid post object.", badRequestResult.Value);
        }

        // Positive test for delete post

        [Fact]
        public async Task DeletePost_WithValidId_ReturnsRedirectToIndex()
        {
            // Arrange
            var postId = 1;
            var post = new Post { Id = postId, UserId = "test-user-id" };

            _mockPostRepository.Setup(repo => repo.GetPostByIdAsync(postId)).ReturnsAsync(post);
            _mockPostRepository.Setup(repo => repo.DeletePostAsync(postId)).Returns(Task.CompletedTask);

            // Act
            var result = await _postController.DeletePost(postId);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            _mockPostRepository.Verify(repo => repo.DeletePostAsync(postId), Times.Once);
        }

        // Negative test for delete post

        [Fact]
        public async Task DeletePost_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            var postId = 99;
            _mockPostRepository.Setup(repo => repo.GetPostByIdAsync(postId)).ReturnsAsync((Post)null);

            // Act
            var result = await _postController.DeletePost(postId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        // Positive test for update post

        [Fact]
        public async Task UpdatePost_GetWithValidId_ReturnsViewWithPost()
        {
            // Arrange
            var postId = 1;
            var post = new Post { Id = postId, Content = "Original Content", UserId = "test-user-id" };

            _mockPostRepository.Setup(repo => repo.GetPostByIdAsync(postId)).ReturnsAsync(post);

            // Act
            var result = await _postController.UpdatePost(postId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(post, viewResult.Model);
        }

        // Negative test for update post

        [Fact]
        public async Task UpdatePost_GetNonExistentPost_ReturnsNotFound()
        {
            // Arrange
            var nonExistentPostId = 999;
            _mockPostRepository.Setup(repo => repo.GetPostByIdAsync(nonExistentPostId)).ReturnsAsync((Post)null);

            // Act
            var result = await _postController.UpdatePost(nonExistentPostId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        // Positive test for add comment

        [Fact]
        public async Task AddComment_WithValidComment_ReturnsRedirectToIndex()
        {
            // Arrange
            var comment = new Comment { Content = "New comment" };
            _mockCommentRepository.Setup(repo => repo.AddCommentAsync(It.IsAny<Comment>())).Returns(Task.CompletedTask);

            var postController = new PostController(_mockPostRepository.Object, _mockCommentRepository.Object, _mockLogger.Object);

            // Sett opp en mock-bruker for testen
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "test-user-id"),
                new Claim(ClaimTypes.Name, "testuser")
            }, "mock"));
            postController.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = user } };

            // Act
            var result = await postController.AddComment(comment);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            _mockCommentRepository.Verify(repo => repo.AddCommentAsync(It.IsAny<Comment>()), Times.Once);
        }

        // Negative test for add comment

        [Fact]
        public async Task AddComment_WithNullComment_ReturnsBadRequest()
        {
            // Arrange
            Comment comment = null;

            // Act
            var result = await _postController.AddComment(comment);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid comment.", badRequestResult.Value);
        }

        // Positive test for update comment
        [Fact]
        public async Task UpdateComment_WithValidIdAndUpdatedComment_ReturnsRedirectToIndex()
        {
            // Arrange
            var commentId = 1;
            var updatedComment = new Comment { Content = "Updated comment" };
            var comment = new Comment { Id = commentId, Content = "Original comment", UserId = "test-user-id" };

            _mockCommentRepository.Setup(repo => repo.GetCommentByIdAsync(commentId)).ReturnsAsync(comment);
            _mockCommentRepository.Setup(repo => repo.UpdateCommentAsync(It.IsAny<Comment>())).Returns(Task.CompletedTask);

            // Act
            var result = await _postController.UpdateComment(commentId, updatedComment);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            _mockCommentRepository.Verify(repo => repo.UpdateCommentAsync(It.IsAny<Comment>()), Times.Once);
        }

        // Negative test for update comment

        [Fact]
        public async Task UpdateComment_WithUnauthorizedUser_ReturnsForbid()
        {
            // Arrange
            var commentId = 1;
            var updatedComment = new Comment { Content = "Updated content" };
            var comment = new Comment { Id = commentId, Content = "Original content", UserId = "other-user-id" }; // Eies av en annen bruker

            _mockCommentRepository.Setup(repo => repo.GetCommentByIdAsync(commentId)).ReturnsAsync(comment);

            // Act
            var result = await _postController.UpdateComment(commentId, updatedComment);

            // Assert
            Assert.IsType<ForbidResult>(result);
        }


        // Positive test for delete comment

        [Fact]
        public async Task DeleteComment_WithValidId_ReturnsRedirectToIndex()
        {
            // Arrange
            var commentId = 1;
            var comment = new Comment { Id = commentId, UserId = "test-user-id" };
            _mockCommentRepository.Setup(repo => repo.GetCommentByIdAsync(commentId)).ReturnsAsync(comment);
            _mockCommentRepository.Setup(repo => repo.DeleteCommentAsync(commentId)).Returns(Task.CompletedTask);

            // Act
            var result = await _postController.DeleteComment(commentId);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            _mockCommentRepository.Verify(repo => repo.DeleteCommentAsync(commentId), Times.Once);
        }

        // Negative test for delete comment

        [Fact]
        public async Task DeleteComment_WithNonExistentCommentId_ReturnsNotFound()
        {
            // Arrange
            var commentId = 99;
            _mockCommentRepository.Setup(repo => repo.GetCommentByIdAsync(commentId)).ReturnsAsync((Comment)null);

            // Act
            var result = await _postController.DeleteComment(commentId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }










    }
}

