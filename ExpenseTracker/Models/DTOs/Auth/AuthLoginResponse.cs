namespace ExpenseTracker.Models.DTOs.Auth;

/// <summary>
/// Represents the response returned upon successful authentication,
/// containing both user identity and authentication token information.
/// </summary>
public class AuthLoginResponse
{
    /// <summary>
    /// Information about the authenticated user.
    /// </summary>
    public required AuthUserDto User { get; set; }

    /// <summary>
    /// Authentication token details for the session.
    /// </summary>
    public required AuthTokenDto Auth { get; set; }
}
