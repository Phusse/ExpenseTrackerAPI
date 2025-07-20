using ExpenseTracker.Contracts;
using ExpenseTracker.Models;
using ExpenseTracker.Models.DTOs.Auth;
using ExpenseTracker.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ExpenseTracker.Controllers;

[ApiController]
public class AuthController(IAuthService authService) : ControllerBase
{
    private readonly IAuthService _authService = authService;

    /// <summary>
    /// Logs a user in and returns a JWT token.
    /// </summary>
    /// <remarks>
    /// This endpoint authenticates a user with their email and password.
    /// On success, it returns a JWT that can be used for future requests.
    /// </remarks>
    /// <param name="request">The login model containing email and password.</param>
    /// <returns>A JWT token if login is successful, otherwise 400 Bad Request.</returns>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPost(ApiRoutes.Auth.Post.Login)]
    public async Task<IActionResult> Login([FromBody] AuthLoginRequest request)
    {
        ServiceResult<AuthLoginResponse?> result = await _authService.LoginAsync(request);

        if (result.IsSuccess)
        {
            return Ok(ApiResponse<object?>.Success(result.Data, "Login successful."));
        }

        return Unauthorized(ApiResponse<object?>.Failure(null, result.Message ?? "Login failed"));
    }

    [HttpPost(ApiRoutes.Auth.Post.Register)]
    public async Task<IActionResult> Register([FromBody] AuthRegisterRequest request)
    {
        ServiceResult<object?> result = await _authService.RegisterAsync(request);

        if (result.IsSuccess)
        {
            return CreatedAtAction(nameof(Register), ApiResponse<object?>.Success(null, "User registered successfully."));
        }

        return BadRequest(ApiResponse<object?>.Failure(null, result.Message ?? "Registration failed."));
    }

    [Authorize]
    [HttpGet(ApiRoutes.Auth.Get.CurrentUser)]
    public async Task<IActionResult> GetCurrentUser()
    {
        Claim? userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim is null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return Unauthorized(ApiResponse<object?>.Failure(null, "Invalid or missing token."));
        }

        User? user = await _authService.GetUserByIdAsync(userId);

        if (user is null)
        {
            return NotFound(ApiResponse<object?>.Failure(null, "User not found."));
        }

        var userInfo = new
        {
            id = user.Id,
            name = user.Name,
            email = user.Email,
            createdAt = user.CreatedAt,
            lastLoginAt = user.LastLoginAt
        };

        return Ok(ApiResponse<object?>.Success(userInfo, "User retrieved successfully."));
    }

    [HttpPost(ApiRoutes.Auth.Post.Logout)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Logout()
    {
        Claim? userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim is null || !Guid.TryParse(userIdClaim.Value, out var userId))
            return Unauthorized(ApiResponse<object?>.Failure(null, "Invalid or missing token."));

        ServiceResult<object?> result = await _authService.LogoutAsync(userId);

        if (result.IsSuccess)
            return Ok(ApiResponse<object?>.Success(null, result.Message));

        return NotFound(ApiResponse<object?>.Failure(null, result.Message));
    }
}