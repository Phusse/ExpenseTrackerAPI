using ExpenseTracker.Models;
using ExpenseTracker.Services;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

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
}