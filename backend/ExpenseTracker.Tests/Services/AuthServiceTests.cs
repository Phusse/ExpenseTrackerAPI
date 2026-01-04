using ExpenseTracker.Data;
using ExpenseTracker.Models;
using ExpenseTracker.Models.DTOs.Auth;
using ExpenseTracker.Services;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ExpenseTracker.Tests.Services;

public class AuthServiceTests
{
    private readonly ExpenseTrackerDbContext _dbContext;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly Mock<IEmailService> _emailServiceMock;
    private readonly Mock<ILogger<AuthService>> _loggerMock;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        var options = new DbContextOptionsBuilder<ExpenseTrackerDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _dbContext = new ExpenseTrackerDbContext(options);

        _configurationMock = new Mock<IConfiguration>();
        _emailServiceMock = new Mock<IEmailService>();
        _loggerMock = new Mock<ILogger<AuthService>>();

        // Setup default JWT settings
        var jwtSectionMock = new Mock<IConfigurationSection>();
        jwtSectionMock.Setup(x => x["SecretKey"]).Returns("SuperSecretKey12345678901234567890");
        jwtSectionMock.Setup(x => x["Issuer"]).Returns("TestIssuer");
        jwtSectionMock.Setup(x => x["Audience"]).Returns("TestAudience");
        _configurationMock.Setup(x => x.GetSection("JwtSettings")).Returns(jwtSectionMock.Object);

        _authService = new AuthService(_dbContext, _configurationMock.Object, _emailServiceMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task RegisterAsync_ShouldRegisterNewUser_WhenEmailIsUnique()
    {
        // Arrange
        var request = new AuthRegisterRequest
        {
            Name = "Test User",
            Email = "test@example.com",
            Password = "Password123!"
        };

        _emailServiceMock.Setup(x => x.SendTemplateEmailAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<object>()))
            .ReturnsAsync(true);

        // Act
        var result = await _authService.RegisterAsync(request);

        // Assert
        result.Success.Should().BeTrue();
        result.Message.Should().Be("Registration successful.");

        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        user.Should().NotBeNull();
        user!.Name.Should().Be(request.Name);
        BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash).Should().BeTrue();
    }

    [Fact]
    public async Task RegisterAsync_ShouldFail_WhenEmailAlreadyExists()
    {
        // Arrange
        var existingUser = new User
        {
            Id = Guid.NewGuid(),
            Name = "Existing User",
            Email = "existing@example.com",
            PasswordHash = "hash",
            IsActive = true
        };
        _dbContext.Users.Add(existingUser);
        await _dbContext.SaveChangesAsync();

        var request = new AuthRegisterRequest
        {
            Name = "Test User",
            Email = "existing@example.com",
            Password = "Password123!"
        };

        // Act
        var result = await _authService.RegisterAsync(request);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Contain("User with this email already exists");
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnToken_WhenCredentialsAreValid()
    {
        // Arrange
        var password = "Password123!";
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = "Test User",
            Email = "login@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        var request = new AuthLoginRequest
        {
            Email = user.Email,
            Password = password
        };

        _emailServiceMock.Setup(x => x.SendTemplateEmailAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<object>()))
            .ReturnsAsync(true);

        // Act
        var result = await _authService.LoginAsync(request);

        // Assert
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Auth.Token.Should().NotBeNullOrEmpty();
        result.Data.User.Email.Should().Be(user.Email);
    }

    [Fact]
    public async Task LoginAsync_ShouldFail_WhenPasswordIsInvalid()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = "Test User",
            Email = "wrongpass@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("CorrectPassword"),
            IsActive = true
        };
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        var request = new AuthLoginRequest
        {
            Email = user.Email,
            Password = "WrongPassword"
        };

        // Act
        var result = await _authService.LoginAsync(request);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Be("Invalid email or password");
    }

    [Fact]
    public async Task LoginAsync_ShouldFail_WhenAccountIsDeactivated()
    {
        // Arrange
        var password = "Password123!";
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = "Test User",
            Email = "inactive@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            IsActive = false
        };
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        var request = new AuthLoginRequest
        {
            Email = user.Email,
            Password = password
        };

        // Act
        var result = await _authService.LoginAsync(request);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Be("Account is deactivated");
    }
}
