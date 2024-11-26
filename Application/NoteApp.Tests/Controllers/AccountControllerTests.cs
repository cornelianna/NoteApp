using Microsoft.AspNetCore.Mvc;
using Moq;
using NoteApp.Controllers;
using NoteApp.Models;
using NoteApp.Repositories;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using IdentitySignInResult = Microsoft.AspNetCore.Identity.SignInResult;


namespace NoteApp.Test.Controllers
{
    public class AccountControllerTests
    {
        private readonly Mock<IAccountRepository> _mockAccountRepository;
        private readonly Mock<ILogger<AccountController>> _mockLogger;
        private readonly AccountController _accountController;

        public AccountControllerTests()
        {
            _mockAccountRepository = new Mock<IAccountRepository>();
            _mockLogger = new Mock<ILogger<AccountController>>();
            _accountController = new AccountController(_mockAccountRepository.Object, _mockLogger.Object);
        }

        // Positive test for successful registration

        [Fact]
        public async Task Register_ValidModel_ReturnsRedirectToIndex()
        {
            // Arrange
            var model = new RegisterViewModel { Username = "testuser", Email = "test@test.com", Password = "Password123!" };
            var mockAccountRepository = new Mock<IAccountRepository>();
            var mockLogger = new Mock<ILogger<AccountController>>();
            var accountController = new AccountController(mockAccountRepository.Object, mockLogger.Object);

            mockAccountRepository.Setup(repo => repo.RegisterAsync(model))
                                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await accountController.Register(model);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Equal("Post", redirectResult.ControllerName);

            // Verifiser at LogInformation ble kalt (uten spesifikk melding)
            mockLogger.Verify(logger => logger.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true), // Ignorerer meldingens innhold her
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Once);
        }

        // Negative test for unsuccesfull registration
        
        [Fact]
        public async Task Register_InvalidModel_AddsErrorsAndReturnsView()
        {
            // Arrange
            var model = new RegisterViewModel { Username = "testuser", Email = "test@test.com", Password = "Password123!" };
            var mockAccountRepository = new Mock<IAccountRepository>();
            var mockLogger = new Mock<ILogger<AccountController>>();
            var accountController = new AccountController(mockAccountRepository.Object, mockLogger.Object);

            // Simulerer mislykket registrering med en feilmelding
            var identityErrors = new[] { new IdentityError { Description = "Username already taken" } };
            mockAccountRepository.Setup(repo => repo.RegisterAsync(model))
                                .ReturnsAsync(IdentityResult.Failed(identityErrors));

            // Act
            var result = await accountController.Register(model);

            // Assert


        }

        // Positive test for successful login

        [Fact]
        public async Task Login_ValidCredentials_ReturnsRedirectToIndex()
        {
            // Arrange
            var model = new LoginViewModel { Username = "testuser", Password = "Password123!", RememberMe = false };
            var mockAccountRepository = new Mock<IAccountRepository>();
            var mockLogger = new Mock<ILogger<AccountController>>();
            var accountController = new AccountController(mockAccountRepository.Object, mockLogger.Object);

            // Simulerer en vellykket innlogging
            mockAccountRepository.Setup(repo => repo.LoginAsync(model))
                                .ReturnsAsync(IdentitySignInResult.Success); // Bruker aliaset her

            // Act
            var result = await accountController.Login(model);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Equal("Post", redirectResult.ControllerName);

            // Verifiser at LogInformation ble kalt ved suksessfull innlogging
            mockLogger.Verify(logger => logger.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception?>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)), Times.Once);
        }

        // Negative test for unsuccessful login

        [Fact]
        public async Task Login_InvalidCredentials_ReturnsViewWithModelError()
        {
            // Arrange
            var model = new LoginViewModel { Username = "testuser", Password = "WrongPassword", RememberMe = false };
            var mockAccountRepository = new Mock<IAccountRepository>();
            var mockLogger = new Mock<ILogger<AccountController>>();
            var accountController = new AccountController(mockAccountRepository.Object, mockLogger.Object);

            // Simulerer en mislykket innlogging
            mockAccountRepository.Setup(repo => repo.LoginAsync(model))
                                .ReturnsAsync(IdentitySignInResult.Failed); // Bruker aliaset her for mislykket resultat

            // Act
            var result = await accountController.Login(model);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(model, viewResult.Model);  // Sjekker at modellen returneres til visningen

            // Sjekker at feilmeldingen ble lagt til i ModelState
            Assert.True(accountController.ModelState.ContainsKey(string.Empty));
            Assert.Equal("Invalid login attempt.", accountController.ModelState[string.Empty].Errors[0].ErrorMessage);

            // Verifiser at LogWarning ble kalt ved mislykket innlogging
            mockLogger.Verify(logger => logger.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),  // Ignorerer meldingens innhold her
                It.IsAny<Exception?>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)), Times.Once);
        }

        // Positive test for successful logout

        [Fact]
        public async Task Logout_RedirectsToIndex()
        {
            // Arrange
            var mockAccountRepository = new Mock<IAccountRepository>();
            var mockLogger = new Mock<ILogger<AccountController>>();
            var accountController = new AccountController(mockAccountRepository.Object, mockLogger.Object);

            // Simulerer en vellykket utlogging
            mockAccountRepository.Setup(repo => repo.LogoutAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await accountController.Logout();

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Equal("Post", redirectResult.ControllerName);

            // Verifiser at LogoutAsync ble kalt én gang
            mockAccountRepository.Verify(repo => repo.LogoutAsync(), Times.Once);

            // Verifiser at LogInformation ble kalt
            mockLogger.Verify(logger => logger.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),  // Ignorerer meldingens innhold her
                It.IsAny<Exception?>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)), Times.Once);
        }

        // Negative test for unsuccessful logout

        [Fact]
        public async Task Logout_WhenExceptionOccurs_RedirectsToError()
        {
            // Arrange
            var mockAccountRepository = new Mock<IAccountRepository>();
            var mockLogger = new Mock<ILogger<AccountController>>();
            var accountController = new AccountController(mockAccountRepository.Object, mockLogger.Object);

            // Simulerer en feil under utlogging
            mockAccountRepository.Setup(repo => repo.LogoutAsync()).ThrowsAsync(new Exception("Logout failed"));

            // Act
            var result = await accountController.Logout();

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Error", redirectResult.ActionName);
            Assert.Equal("Error", redirectResult.ControllerName);

            // Verifiser at LogError ble kalt
            mockLogger.Verify(logger => logger.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("An unexpected error occurred during logout")),  // Kontrollerer meldingens innhold
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Once);
        }

    }
}
