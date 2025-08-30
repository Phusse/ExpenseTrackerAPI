using ExpenseTracker.Models;
using ExpenseTracker.Models.DTOs.Auth;
using ExpenseTracker.Services;
using ExpenseTracker.Utilities.Extension;
using ExpenseTracker.Utilities.Routing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Controllers;

/// <summary>
/// Provides authentication endpoints for user login, registration, profile retrieval, and logout.
/// </summary>
[ApiController]
public class AuthController(IAuthService authService) : ControllerBase
{
    private readonly IAuthService _authService = authService;

    /// <summary>
    /// Logs a user in and returns a JWT token.
    /// </summary>
    /// <remarks>
    /// This endpoint authenticates a user with their email and password.
    /// On success, it returns a JWT token that must be used in the `Authorization` header
    /// for accessing protected routes.
    /// </remarks>
    /// <param name="request">The login credentials (email and password).</param>
    /// <returns>Returns a JWT token if login is successful.</returns>
    /// <response code="200">Login successful. JWT returned in response.</response>
    /// <response code="400">Login failed due to validation or business logic issues.</response>
    /// <response code="401">Login failed due to invalid credentials.</response>
    [ProducesResponseType(typeof(ApiResponse<AuthLoginResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status401Unauthorized)]
    [HttpPost(ApiRoutes.Auth.Post.Login)]
    public async Task<IActionResult> Login([FromBody] AuthLoginRequest request)
    {
        ServiceResult<AuthLoginResponse?> result = await _authService.LoginAsync(request);

        if (result.Success)
        {
            return Ok(ApiResponse<AuthLoginResponse?>.Ok(result.Data, "Login successful."));
        }

        return Unauthorized(ApiResponse<object?>.Fail(null, result.Message ?? "Login failed."));
    }

    /// <summary>
    /// Registers a new user.
    /// </summary>
    /// <remarks>
    /// Creates a new user account with the provided registration information.
    /// </remarks>
    /// <param name="request">The registration details (name, email, password).</param>
    /// <returns>A success message upon successful registration.</returns>
    /// <response code="201">User registered successfully.</response>
    /// <response code="400">Registration failed due to validation or business logic issues.</response>
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status400BadRequest)]
    [HttpPost(ApiRoutes.Auth.Post.Register)]
    public async Task<IActionResult> Register([FromBody] AuthRegisterRequest request)
    {
        ServiceResult<object?> result = await _authService.RegisterAsync(request);

        if (result.Success)
        {
            return CreatedAtAction(nameof(Register), ApiResponse<object?>.Ok(null, result.Message, result.Errors));
        }

        return BadRequest(ApiResponse<object?>.Fail(null, result.Message ?? "Registration failed."));
    }

    /// <summary>
    /// Gets information of the current authenticated user.
    /// </summary>
    /// <remarks>
    /// This endpoint returns user profile data for the current logged-in user.
    /// It requires a valid JWT in the `Authorization` header.
    /// </remarks>
    /// <returns>Returns user ID, name, email, and other profile information.</returns>
    /// <response code="200">User profile retrieved successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="404">User ID in the token does not correspond to any user.</response>
    [ProducesResponseType(typeof(ApiResponse<UserProfileResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status404NotFound)]
    [Authorize]
    [HttpGet(ApiRoutes.Auth.Get.CurrentUser)]
    public async Task<IActionResult> GetCurrentUser()
    {
        if (!User.TryGetUserId(out Guid userId))
        {
            return Unauthorized(ApiResponse<object?>.Fail(null, "Invalid user token."));
        }

        UserProfileResponse? user = await _authService.GetUserProfileByIdAsync(userId);

        if (user is null)
        {
            return NotFound(ApiResponse<object?>.Fail(null, "User not found."));
        }

        return Ok(ApiResponse<UserProfileResponse>.Ok(user, "User retrieved successfully."));
    }

    /// <summary>
    /// Logs the current user out.
    /// </summary>
    /// <remarks>
    /// Logs the user out from the application. JWT tokens are stateless,
    /// so this may invalidate refresh tokens or update user login state.
    /// </remarks>
    /// <returns>A confirmation message.</returns>
    /// <response code="200">User logged out successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="404">User not found or already logged out.</response>
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status404NotFound)]
    [Authorize]
    [HttpPost(ApiRoutes.Auth.Post.Logout)]
    public async Task<IActionResult> Logout()
    {
        if (!User.TryGetUserId(out Guid userId))
        {
            return Unauthorized(ApiResponse<object?>.Fail(null, "Invalid or missing token."));
        }

        ServiceResult<object?> result = await _authService.LogoutAsync(userId);

        if (result.Success)
        {
            return Ok(ApiResponse<object?>.Ok(null, result.Message));
        }

        return NotFound(ApiResponse<object?>.Fail(null, result.Message));
    }
}
