using ExpenseTracker.Models;
using ExpenseTracker.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ExpenseTracker.Controllers;

[ApiController]
[Route("api/v1/auth")]
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
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new AuthResponse
            {
                Success = false,
                Message = "Invalid input data",
                Data = null
            });
        }

        var result = await _authService.LoginAsync(request);

        if (!result.IsSuccess)
        {
            return Unauthorized(new AuthResponse
            {
                Success = false,
                Message = result.ErrorMessage ?? "Login failed",
                Data = null
            });
        }

        return Ok(new AuthResponse
        {
            Success = true,
            Message = "Login successful",
            Data = result.Data
        });
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new AuthResponse
            {
                Success = false,
                Message = "Invalid input data",
                Data = null
            });
        }

        var result = await _authService.RegisterAsync(request);

        if (!result.IsSuccess)
        {
            return BadRequest(new AuthResponse
            {
                Success = false,
                Message = result.ErrorMessage ?? "Registration failed",
                Data = null
            });
        }

        return CreatedAtAction(nameof(Register), new AuthResponse
        {
            Success = true,
            Message = "User registered successfully",
            Data = null
        });
    }

    [HttpGet("me")]
    [Microsoft.AspNetCore.Authorization.Authorize]
    public async Task<IActionResult> GetCurrentUser()
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return Unauthorized();
        }

        var user = await _authService.GetUserByIdAsync(userId);
        if (user == null)
        {
            return NotFound();
        }

        return Ok(new
        {
            success = true,
            data = new
            {
                id = user.Id,
                name = user.Name,
                email = user.Email,
                createdAt = user.CreatedAt,
                lastLoginAt = user.LastLoginAt
            }
        });
    }
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            return Unauthorized(new { message = "Invalid token" });

        var result = await _authService.LogoutAsync(userId);
        if (!result.IsSuccess)
            return NotFound(new { message = result.Message });

        return Ok(new { message = result.Message });
    }
}