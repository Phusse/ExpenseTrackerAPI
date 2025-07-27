namespace ExpenseTracker.Models.DTOs.Auth;

/// <summary>
/// Represents the response model for a user's profile information.
/// </summary>
public class UserProfileResponse
{
	/// <summary>
	/// The unique identifier of the user.
	/// </summary>
	public required Guid Id { get; set; }

	/// <summary>
	/// The full name of the user.
	/// </summary>
	public required string Name { get; set; }

	/// <summary>
	/// The email address of the user.
	/// </summary>
	public required string Email { get; set; }

	/// <summary>
	/// The date and time when the user account was created.
	/// </summary>
	public required DateTime CreatedAt { get; set; }

	/// <summary>
	/// The date and time when the user last logged in.
	/// </summary>
	public required DateTime? LastLoginAt { get; set; }
}
