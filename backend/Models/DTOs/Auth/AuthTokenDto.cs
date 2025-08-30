namespace ExpenseTracker.Models.DTOs.Auth;

/// <summary>
/// Contains basic identity information for the authenticated user.
/// </summary>
public class AuthUserDto
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
	/// The email address associated with the user's account.
	/// </summary>
	public required string Email { get; set; }
}
