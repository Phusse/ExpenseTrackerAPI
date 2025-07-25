using ExpenseTracker.Models;
using ExpenseTracker.Models.DTOs.Auth;

namespace ExpenseTracker.Services;

/// <summary>
/// Defines authentication-related operations such as login, registration, and logout.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Attempts to log in a user using the provided credentials.
    /// </summary>
    /// <param name="request">The login request containing user credentials.</param>
    /// <returns>The <see cref="AuthLoginResponse"/> if credentials are valid; otherwise, null.</returns>
    Task<ServiceResult<AuthLoginResponse?>> LoginAsync(AuthLoginRequest request);

    /// <summary>
    /// Registers a new user with the provided information.
    /// </summary>
    /// <param name="request">The registration request containing user details.</param>
    /// <returns>The created <see cref="User"/> if registration succeeds; otherwise, null.</returns>
    Task<ServiceResult<object?>> RegisterAsync(AuthRegisterRequest request);

    /// <summary>
    /// Retrieves a user profile by their unique identifier.
    /// </summary>
    /// <param name="userId">The ID of the user to retrieve.</param>
    /// <returns>The corresponding <see cref="UserProfileResponse"/>, or null if not found.</returns>
    Task<UserProfileResponse?> GetUserProfileByIdAsync(Guid userId);

    /// <summary>
    /// Retrieves a user by their email address.
    /// </summary>
    /// <param name="email">The email address of the user to retrieve.</param>
    /// <returns>The corresponding <see cref="User"/>, or null if not found.</returns>
    Task<User?> GetUserByEmailAsync(string email);

    /// <summary>
    /// Logs out the specified user.
    /// </summary>
    /// <param name="userId">The ID of the user to log out.</param>
    /// <returns><c>true</c> if logout was successful; otherwise, <c>false</c>.</returns>
    Task<ServiceResult<object?>> LogoutAsync(Guid userId);
}
