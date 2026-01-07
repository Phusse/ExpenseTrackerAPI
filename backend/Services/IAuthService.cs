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

    /// <summary>
    /// Updates user profile information.
    /// </summary>
    Task<ServiceResult<UserProfileResponse>> UpdateProfileAsync(Guid userId, string? name, string? email);

    /// <summary>
    /// Changes user password.
    /// </summary>
    Task<ServiceResult<object?>> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword);

    /// <summary>
    /// Registers a new user with security questions.
    /// </summary>
    Task<ServiceResult<object?>> RegisterWithSecurityQuestionsAsync(AuthRegisterWithSecurityRequest request);

    /// <summary>
    /// Gets user's security questions for password reset (no answers).
    /// </summary>
    Task<ServiceResult<ForgotPasswordQuestionsResponse>> GetSecurityQuestionsForResetAsync(string email);

    /// <summary>
    /// Gets the current user's security questions.
    /// </summary>
    Task<ServiceResult<List<UserSecurityQuestion>>> GetMySecurityQuestionsAsync(Guid userId);

    /// <summary>
    /// Verifies security answers and resets password.
    /// </summary>
    Task<ServiceResult<object?>> ResetPasswordWithSecurityQuestionsAsync(ResetPasswordRequest request);

    /// <summary>
    /// Gets list of all available security questions.
    /// </summary>
    SecurityQuestionsListResponse GetAvailableSecurityQuestions();
}
