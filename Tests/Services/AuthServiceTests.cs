using System.Security.Claims;
using arabia.DTOs.Requests;
using arabia.DTOs.Responses;
using arabia.Models;
using arabia.Services;
using arabia.Services.Interfaces;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;

namespace arabia.Tests.Services;

[TestFixture]
public class AuthServiceTests
{
    private Mock<UserManager<User>> _mockUserManager = null!;
    private Mock<RoleManager<Role>> _mockRoleManager = null!;
    private Mock<SignInManager<User>> _mockSignInManager = null!;
    private Mock<IConfiguration> _mockConfiguration = null!;
    private Mock<IMapper> _mockMapper = null!;
    private Mock<arabia.Services.Interfaces.IUserRepository> _mockUserRepository = null!;
    private AuthService _authService = null!;

    [SetUp]
    public void Setup()
    {
        var userStore = new Mock<IUserStore<User>>();
        _mockUserManager = new Mock<UserManager<User>>(
            userStore.Object,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!
        );

        var roleStore = new Mock<IRoleStore<Role>>();
        _mockRoleManager = new Mock<RoleManager<Role>>(
            roleStore.Object,
            null!,
            null!,
            null!,
            null!
        );

        var contextAccessor = new Mock<Microsoft.AspNetCore.Http.IHttpContextAccessor>();
        var userPrincipalFactory = new Mock<IUserClaimsPrincipalFactory<User>>();
        var options = new Mock<Microsoft.Extensions.Options.IOptions<IdentityOptions>>();
        var logger = new Mock<Microsoft.Extensions.Logging.ILogger<SignInManager<User>>>();
        var schemes = new Mock<Microsoft.AspNetCore.Authentication.IAuthenticationSchemeProvider>();
        var confirmation = new Mock<IUserConfirmation<User>>();

        _mockSignInManager = new Mock<SignInManager<User>>(
            _mockUserManager.Object,
            contextAccessor.Object,
            userPrincipalFactory.Object,
            options.Object,
            logger.Object,
            schemes.Object,
            confirmation.Object
        );

        _mockConfiguration = new Mock<IConfiguration>();
        _mockMapper = new Mock<IMapper>();
        _mockUserRepository = new Mock<arabia.Services.Interfaces.IUserRepository>();

        // Setup JWT configuration
        _mockConfiguration
            .Setup(c => c["Jwt:Key"])
            .Returns("YourSuperSecretKeyThatIsAtLeast32CharactersLong!");
        _mockConfiguration.Setup(c => c["Jwt:Issuer"]).Returns("TestIssuer");
        _mockConfiguration.Setup(c => c["Jwt:Audience"]).Returns("TestAudience");

        _authService = new AuthService(
            _mockUserManager.Object,
            _mockRoleManager.Object,
            _mockSignInManager.Object,
            _mockConfiguration.Object,
            _mockMapper.Object,
            _mockUserRepository.Object
        );
    }

    [Test]
    public async Task LoginAsync_WhenUserExistsAndPasswordCorrect_ShouldReturnAuthResponse()
    {
        // Arrange
        var request = new LoginRequest { Email = "test@example.com", Password = "Password123!" };
        var user = new User
        {
            Id = 1,
            Email = request.Email,
            UserName = request.Email,
            FirstName = "Test",
            LastName = "User",
            IsActive = true,
        };

        _mockUserManager.Setup(m => m.FindByEmailAsync(request.Email)).ReturnsAsync(user);
        _mockSignInManager
            .Setup(m => m.CheckPasswordSignInAsync(user, request.Password, false))
            .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);
        _mockUserManager
            .Setup(m => m.GetRolesAsync(user))
            .ReturnsAsync(new List<string> { "User" });
        _mockUserManager.Setup(m => m.UpdateAsync(user)).ReturnsAsync(IdentityResult.Success);

        var userResponse = new UserResponse
        {
            Id = user.Id,
            Email = user.Email!,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Roles = new List<string> { "User" },
        };
        _mockMapper.Setup(m => m.Map<UserResponse>(user)).Returns(userResponse);

        // Act
        var result = await _authService.LoginAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Token.Should().NotBeNullOrEmpty();
        result.RefreshToken.Should().NotBeNullOrEmpty();
        result.User.Should().NotBeNull();
        result.User.Email.Should().Be(request.Email);
        _mockUserManager.Verify(m => m.UpdateAsync(It.IsAny<User>()), Times.Once);
    }

    [Test]
    public void LoginAsync_WhenUserNotFound_ShouldThrowUnauthorizedAccessException()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "notfound@example.com",
            Password = "Password123!",
        };
        _mockUserManager.Setup(m => m.FindByEmailAsync(request.Email)).ReturnsAsync((User?)null);

        // Act & Assert
        AsyncTestDelegate act = async () => await _authService.LoginAsync(request);
        UnauthorizedAccessException exception = Assert.ThrowsAsync<UnauthorizedAccessException>(
            act
        );
        Assert.That(exception, Is.Not.Null);
        exception.Message.Should().Contain("Invalid email or password");
    }

    [Test]
    public void LoginAsync_WhenUserInactive_ShouldThrowUnauthorizedAccessException()
    {
        // Arrange
        var request = new LoginRequest { Email = "test@example.com", Password = "Password123!" };
        var user = new User
        {
            Id = 1,
            Email = request.Email,
            IsActive = false,
        };
        _mockUserManager.Setup(m => m.FindByEmailAsync(request.Email)).ReturnsAsync(user);

        // Act & Assert
        AsyncTestDelegate act = async () => await _authService.LoginAsync(request);
        UnauthorizedAccessException exception = Assert.ThrowsAsync<UnauthorizedAccessException>(
            act
        );
        Assert.That(exception, Is.Not.Null);
        exception.Message.Should().Contain("Invalid email or password");
    }

    [Test]
    public void LoginAsync_WhenPasswordIncorrect_ShouldThrowUnauthorizedAccessException()
    {
        // Arrange
        var request = new LoginRequest { Email = "test@example.com", Password = "WrongPassword" };
        var user = new User
        {
            Id = 1,
            Email = request.Email,
            IsActive = true,
        };
        _mockUserManager.Setup(m => m.FindByEmailAsync(request.Email)).ReturnsAsync(user);
        _mockSignInManager
            .Setup(m => m.CheckPasswordSignInAsync(user, request.Password, false))
            .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Failed);

        // Act & Assert
        AsyncTestDelegate act = async () => await _authService.LoginAsync(request);
        UnauthorizedAccessException exception = Assert.ThrowsAsync<UnauthorizedAccessException>(
            act
        );
        Assert.That(exception, Is.Not.Null);
        exception.Message.Should().Contain("Invalid email or password");
    }

    [Test]
    public async Task RegisterAsync_WhenUserDoesNotExist_ShouldReturnAuthResponse()
    {
        // Arrange
        var request = new RegisterRequest
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com",
            Password = "Password123!",
            Role = "User",
        };

        _mockUserManager.Setup(m => m.FindByEmailAsync(request.Email)).ReturnsAsync((User?)null);
        _mockRoleManager.Setup(m => m.RoleExistsAsync(request.Role!)).ReturnsAsync(true);
        _mockUserManager
            .Setup(m => m.CreateAsync(It.IsAny<User>(), request.Password))
            .ReturnsAsync(IdentityResult.Success);
        _mockUserManager
            .Setup(m => m.AddToRoleAsync(It.IsAny<User>(), request.Role!))
            .ReturnsAsync(IdentityResult.Success);
        _mockUserManager
            .Setup(m => m.GetRolesAsync(It.IsAny<User>()))
            .ReturnsAsync(new List<string> { request.Role! });
        _mockUserManager
            .Setup(m => m.UpdateAsync(It.IsAny<User>()))
            .ReturnsAsync(IdentityResult.Success);

        var userResponse = new UserResponse
        {
            Id = 1,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Roles = new List<string> { request.Role! },
        };
        _mockMapper.Setup(m => m.Map<UserResponse>(It.IsAny<User>())).Returns(userResponse);

        // Act
        var result = await _authService.RegisterAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Token.Should().NotBeNullOrEmpty();
        result.RefreshToken.Should().NotBeNullOrEmpty();
        result.User.Should().NotBeNull();
        result.User.Email.Should().Be(request.Email);
        _mockUserManager.Verify(m => m.CreateAsync(It.IsAny<User>(), request.Password), Times.Once);
    }

    [Test]
    public void RegisterAsync_WhenUserAlreadyExists_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var request = new RegisterRequest
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "existing@example.com",
            Password = "Password123!",
        };
        var existingUser = new User { Id = 1, Email = request.Email };
        _mockUserManager.Setup(m => m.FindByEmailAsync(request.Email)).ReturnsAsync(existingUser);

        // Act & Assert
        AsyncTestDelegate act = async () => await _authService.RegisterAsync(request);
        InvalidOperationException exception = Assert.ThrowsAsync<InvalidOperationException>(act);
        Assert.That(exception, Is.Not.Null);
        exception.Message.Should().Contain("already exists");
    }

    [Test]
    public async Task RegisterAsync_WhenRoleDoesNotExist_ShouldCreateRole()
    {
        // Arrange
        var request = new RegisterRequest
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com",
            Password = "Password123!",
            Role = "NewRole",
        };

        _mockUserManager.Setup(m => m.FindByEmailAsync(request.Email)).ReturnsAsync((User?)null);
        _mockRoleManager.Setup(m => m.RoleExistsAsync(request.Role!)).ReturnsAsync(false);
        _mockRoleManager
            .Setup(m => m.CreateAsync(It.IsAny<Role>()))
            .ReturnsAsync(IdentityResult.Success);
        _mockUserManager
            .Setup(m => m.CreateAsync(It.IsAny<User>(), request.Password))
            .ReturnsAsync(IdentityResult.Success);
        _mockUserManager
            .Setup(m => m.AddToRoleAsync(It.IsAny<User>(), request.Role!))
            .ReturnsAsync(IdentityResult.Success);
        _mockUserManager
            .Setup(m => m.GetRolesAsync(It.IsAny<User>()))
            .ReturnsAsync(new List<string> { request.Role! });
        _mockUserManager
            .Setup(m => m.UpdateAsync(It.IsAny<User>()))
            .ReturnsAsync(IdentityResult.Success);

        var userResponse = new UserResponse
        {
            Id = 1,
            Email = request.Email,
            Roles = new List<string> { request.Role! },
        };
        _mockMapper.Setup(m => m.Map<UserResponse>(It.IsAny<User>())).Returns(userResponse);

        // Act
        var result = await _authService.RegisterAsync(request);

        // Assert
        result.Should().NotBeNull();
        _mockRoleManager.Verify(m => m.CreateAsync(It.IsAny<Role>()), Times.Once);
    }

    [Test]
    public async Task RegisterAsync_WhenNoRoleProvided_ShouldDefaultToUser()
    {
        // Arrange
        var request = new RegisterRequest
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com",
            Password = "Password123!",
            Role = null,
        };

        _mockUserManager.Setup(m => m.FindByEmailAsync(request.Email)).ReturnsAsync((User?)null);
        _mockRoleManager.Setup(m => m.RoleExistsAsync("User")).ReturnsAsync(true);
        _mockUserManager
            .Setup(m => m.CreateAsync(It.IsAny<User>(), request.Password))
            .ReturnsAsync(IdentityResult.Success);
        _mockUserManager
            .Setup(m => m.AddToRoleAsync(It.IsAny<User>(), "User"))
            .ReturnsAsync(IdentityResult.Success);
        _mockUserManager
            .Setup(m => m.GetRolesAsync(It.IsAny<User>()))
            .ReturnsAsync(new List<string> { "User" });
        _mockUserManager
            .Setup(m => m.UpdateAsync(It.IsAny<User>()))
            .ReturnsAsync(IdentityResult.Success);

        var userResponse = new UserResponse
        {
            Id = 1,
            Email = request.Email,
            Roles = new List<string> { "User" },
        };
        _mockMapper.Setup(m => m.Map<UserResponse>(It.IsAny<User>())).Returns(userResponse);

        // Act
        var result = await _authService.RegisterAsync(request);

        // Assert
        result.Should().NotBeNull();
        _mockUserManager.Verify(m => m.AddToRoleAsync(It.IsAny<User>(), "User"), Times.Once);
    }

    [Test]
    public async Task RefreshTokenAsync_WhenTokenValid_ShouldReturnNewAuthResponse()
    {
        // Arrange
        var refreshToken = "valid-refresh-token";
        var user = new User
        {
            Id = 1,
            Email = "test@example.com",
            RefreshToken = refreshToken,
            RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(1),
        };

        _mockUserRepository.Setup(r => r.FindByRefreshTokenAsync(refreshToken)).ReturnsAsync(user);
        _mockUserManager
            .Setup(m => m.GetRolesAsync(user))
            .ReturnsAsync(new List<string> { "User" });
        _mockUserManager.Setup(m => m.UpdateAsync(user)).ReturnsAsync(IdentityResult.Success);

        var userResponse = new UserResponse
        {
            Id = 1,
            Email = user.Email!,
            Roles = new List<string> { "User" },
        };
        _mockMapper.Setup(m => m.Map<UserResponse>(user)).Returns(userResponse);

        // Act
        var result = await _authService.RefreshTokenAsync(refreshToken);

        // Assert
        result.Should().NotBeNull();
        result.Token.Should().NotBeNullOrEmpty();
        result.RefreshToken.Should().NotBeNullOrEmpty();
        result.RefreshToken.Should().NotBe(refreshToken); // Should be a new token
        _mockUserManager.Verify(m => m.UpdateAsync(It.IsAny<User>()), Times.Once);
    }

    [Test]
    public void RefreshTokenAsync_WhenTokenNotFound_ShouldThrowUnauthorizedAccessException()
    {
        // Arrange
        var refreshToken = "invalid-token";
        _mockUserRepository
            .Setup(r => r.FindByRefreshTokenAsync(refreshToken))
            .ReturnsAsync((User?)null);

        // Act & Assert
        AsyncTestDelegate act = async () => await _authService.RefreshTokenAsync(refreshToken);
        UnauthorizedAccessException exception = Assert.ThrowsAsync<UnauthorizedAccessException>(
            act
        );
        Assert.That(exception, Is.Not.Null);
        exception.Message.Should().Contain("Invalid or expired refresh token");
    }

    [Test]
    public void RefreshTokenAsync_WhenTokenExpired_ShouldThrowUnauthorizedAccessException()
    {
        // Arrange
        var refreshToken = "expired-token";
        var user = new User
        {
            Id = 1,
            Email = "test@example.com",
            RefreshToken = refreshToken,
            RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(-1), // Expired
        };

        _mockUserRepository.Setup(r => r.FindByRefreshTokenAsync(refreshToken)).ReturnsAsync(user);

        // Act & Assert
        AsyncTestDelegate act = async () => await _authService.RefreshTokenAsync(refreshToken);
        UnauthorizedAccessException exception = Assert.ThrowsAsync<UnauthorizedAccessException>(
            act
        );
        Assert.That(exception, Is.Not.Null);
        exception.Message.Should().Contain("Invalid or expired refresh token");
    }

    [Test]
    public async Task LogoutAsync_WhenTokenExists_ShouldReturnTrue()
    {
        // Arrange
        var refreshToken = "valid-token";
        var user = new User
        {
            Id = 1,
            Email = "test@example.com",
            RefreshToken = refreshToken,
        };

        _mockUserRepository.Setup(r => r.FindByRefreshTokenAsync(refreshToken)).ReturnsAsync(user);
        _mockUserManager
            .Setup(m => m.UpdateAsync(It.IsAny<User>()))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _authService.LogoutAsync(refreshToken);

        // Assert
        result.Should().BeTrue();
        _mockUserManager.Verify(
            m => m.UpdateAsync(It.Is<User>(u => u.RefreshToken == null)),
            Times.Once
        );
    }

    [Test]
    public async Task LogoutAsync_WhenTokenNotFound_ShouldReturnFalse()
    {
        // Arrange
        var refreshToken = "invalid-token";
        _mockUserRepository
            .Setup(r => r.FindByRefreshTokenAsync(refreshToken))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _authService.LogoutAsync(refreshToken);

        // Assert
        result.Should().BeFalse();
        _mockUserManager.Verify(m => m.UpdateAsync(It.IsAny<User>()), Times.Never);
    }
}
