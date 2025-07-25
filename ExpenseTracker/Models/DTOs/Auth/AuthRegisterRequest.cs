using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Models.DTOs.Auth;

/// <summary>
/// Request payload for registering a new user.
/// </summary>
public class AuthRegisterRequest
{
	/// <summary>
	/// Full name of the user.
	/// </summary>
	[Required(ErrorMessage = "Name is required.")]
	[StringLength(100, ErrorMessage = "Name must not exceed 100 characters.")]
	public required string Name { get; set; }

	/// <summary>
	/// Email address of the user.
	/// </summary>
	[Required(ErrorMessage = "Email is required.")]
	[EmailAddress(ErrorMessage = "Invalid email format.")]
	public required string Email { get; set; }

	/// <summary>
	/// Password for the new account (minimum 6 characters).
	/// </summary>
	[Required(ErrorMessage = "Password is required.")]
	[MinLength(6, ErrorMessage = "Password must be at least 6 characters.")]
	public required string Password { get; set; }
}
