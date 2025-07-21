namespace ExpenseTracker.Models.DTOs.Auth;

/// <summary>
/// Response payload returned upon successful authentication.
/// </summary>
public class AuthLoginResponse
{
    /// <summary>
    /// The unique identifier of the authenticated user.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The full name of the user.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The email address of the user.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// The JWT token issued for the authenticated session.
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// The date and time when the token expires.
    /// </summary>
    public DateTime ExpiresAt { get; set; }
}
