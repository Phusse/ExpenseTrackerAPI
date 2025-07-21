namespace ExpenseTracker.Models.DTOs.Auth;

/// <summary>
/// Response payload returned upon successful authentication.
/// </summary>
public class AuthLoginResponse
{
    /// <summary>
    /// The unique identifier of the authenticated user.
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
    /// The JWT token issued for the authenticated session.
    /// </summary>
    public required string Token { get; set; }

    /// <summary>
    /// The date and time when the token expires.
    /// </summary>
    public required DateTime ExpiresAt { get; set; }
}
