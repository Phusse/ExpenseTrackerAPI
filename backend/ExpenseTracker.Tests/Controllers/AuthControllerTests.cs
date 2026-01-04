using ExpenseTracker.Controllers;
using ExpenseTracker.Models;
using ExpenseTracker.Models.DTOs.Auth;
using ExpenseTracker.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace ExpenseTracker.Tests.Controllers;

public class AuthControllerTests
{
    private readonly Mock<IAuthService> _authServiceMock;
    private readonly AuthController _authController;

    public AuthControllerTests()
    {
        _authServiceMock = new Mock<IAuthService>();
        _authController = new AuthController(_authServiceMock.Object);
    }

    [Fact]
    public async Task Register_ShouldReturnOk_WhenRegistrationIsSuccessful()
    {
        // Arrange
        var request = new AuthRegisterRequest
        {
            Name = "Test User",
            Email = "test@example.com",
            Password = "Password123!"
        };

        _authServiceMock.Setup(x => x.RegisterAsync(request))
            .ReturnsAsync(ServiceResult<object?>.Ok(null, "Registration successful."));

        // Act
        var result = await _authController.Register(request);

        // Assert
        var createdResult = result as CreatedAtActionResult;
        createdResult.Should().NotBeNull();
        createdResult!.StatusCode.Should().Be(201);

        var response = createdResult.Value as ApiResponse<object?>;
        response.Should().NotBeNull();
        response!.Success.Should().BeTrue();
        response.Message.Should().Be("Registration successful.");
    }

    [Fact]
    public async Task Register_ShouldReturnBadRequest_WhenRegistrationFails()
    {
        // Arrange
        var request = new AuthRegisterRequest
        {
            Name = "Test User",
            Email = "test@example.com",
            Password = "Password123!"
        };

        _authServiceMock.Setup(x => x.RegisterAsync(request))
            .ReturnsAsync(ServiceResult<object?>.Fail(null, "Registration failed."));

        // Act
        var result = await _authController.Register(request);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);

        var response = badRequestResult.Value as ApiResponse<object?>;
        response.Should().NotBeNull();
        response!.Success.Should().BeFalse();
        response.Message.Should().Be("Registration failed.");
    }

    [Fact]
    public async Task Login_ShouldReturnOk_WhenLoginIsSuccessful()
    {
        // Arrange
        var request = new AuthLoginRequest
        {
            Email = "test@example.com",
            Password = "Password123!"
        };

        var authData = new AuthLoginResponse
        {
            User = new AuthUserDto { Id = Guid.NewGuid(), Name = "Test User", Email = "test@example.com" },
            Auth = new AuthTokenDto { Token = "valid-token", ExpiresAt = DateTime.UtcNow.AddHours(1) }
        };

        _authServiceMock.Setup(x => x.LoginAsync(request))
            .ReturnsAsync(ServiceResult<AuthLoginResponse?>.Ok(authData, "Login successful."));

        // Act
        var result = await _authController.Login(request);

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);

        var response = okResult.Value as ApiResponse<AuthLoginResponse?>;
        response.Should().NotBeNull();
        response!.Success.Should().BeTrue();
        response.Data.Should().Be(authData);
    }

    [Fact]
    public async Task Login_ShouldReturnBadRequest_WhenLoginFails()
    {
        // Arrange
        var request = new AuthLoginRequest
        {
            Email = "test@example.com",
            Password = "WrongPassword"
        };

        _authServiceMock.Setup(x => x.LoginAsync(request))
            .ReturnsAsync(ServiceResult<AuthLoginResponse?>.Fail(null, "Invalid email or password"));

        // Act
        var result = await _authController.Login(request);

        // Assert
        var unauthorizedResult = result as UnauthorizedObjectResult;
        unauthorizedResult.Should().NotBeNull();
        unauthorizedResult!.StatusCode.Should().Be(401);

        var response = unauthorizedResult.Value as ApiResponse<object?>;
        response.Should().NotBeNull();
        response!.Success.Should().BeFalse();
        response.Message.Should().Be("Invalid email or password");
    }
}
