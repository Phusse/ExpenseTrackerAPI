namespace ExpenseTracker.Models.DTOs.Auth;

/// <summary>
/// Represents the issued authentication token and its expiration information.
/// </summary>
public class AuthTokenDto
{
	/// <summary>
	/// The JWT token generated for the session.
	/// </summary>
	public required string Token { get; set; }

	/// <summary>
	/// The UTC date and time when the token will expire.
	/// </summary>
	public required DateTime ExpiresAt { get; set; }
}
