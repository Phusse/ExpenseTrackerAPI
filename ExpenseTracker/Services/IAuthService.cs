using ExpenseTracker.Models;

namespace ExpenseTracker.Services;

public interface IAuthService
{
    Task<(bool IsSuccess, AuthData? Data, string? ErrorMessage)> LoginAsync(LoginRequest request);
    Task<(bool IsSuccess, User? Data, string? ErrorMessage)> RegisterAsync(RegisterRequest request);
    Task<User?> GetUserByIdAsync(Guid userId);
    Task<User?> GetUserByEmailAsync(string email);
    Task<(bool IsSuccess, string? Message)> LogoutAsync(Guid userId);
}