using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Models.DTOs.Auth;

/// <summary>
/// DTO for user login within the authentication context.
/// </summary>
public class AuthLoginRequest
{
    /// <summary>
    /// The email address used to log in.
    /// </summary>
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    public required string Email { get; set; }

    /// <summary>
    /// The user's password (must be at least 6 characters).
    /// </summary>
    [Required(ErrorMessage = "Password is required.")]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters.")]
    public required string Password { get; set; }
}
