using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ExpenseTracker.Data;
using ExpenseTracker.Models;
using ExpenseTracker.Models.DTOs.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Encryption = BCrypt.Net;

namespace ExpenseTracker.Services;

/// <summary>
/// Provides authentication services, including user management and email verification, using the specified database context, configuration, email service, and logger.
/// </summary>
internal class AuthService(ExpenseTrackerDbContext dbContext, IConfiguration configuration, IEmailService emailService, ILogger<AuthService> logger) : IAuthService
{
    private readonly ExpenseTrackerDbContext _dbContext = dbContext;
    private readonly IConfiguration _configuration = configuration;
    private readonly IEmailService _emailService = emailService;
    private readonly ILogger<AuthService> _logger = logger;

    public async Task<ServiceResult<AuthLoginResponse?>> LoginAsync(AuthLoginRequest request)
    {
        try
        {
            User? user = await GetUserByEmailAsync(request.Email);

            if (user is null || !VerifyPassword(request.Password, user.PasswordHash))
            {
                return ServiceResult<AuthLoginResponse?>.Fail(null, "Invalid email or password");
            }

            if (!user.IsActive)
            {
                return ServiceResult<AuthLoginResponse?>.Fail(null, "Account is deactivated");
            }

            user.LastLoginAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();

            try
            {
                var payLoad = new
                {
                    UserName = user.Name,
                    LoginTime = user.LastLoginAt?.ToString("f"),
                    CurrentYear = DateTime.Now.Year
                };

                await _emailService.SendTemplateEmailAsync(
                    to: user.Email,
                    templateId: 40597432,
                    templateModel: payLoad
                );
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Failed to send welcome email: {ex.Message}", ex.Message);
            }

            string token = GenerateJwtToken(user);
            AuthLoginResponse authData = new()
            {
                User = new AuthUserDto
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email
                },
                Auth = new AuthTokenDto
                {
                    Token = token,
                    ExpiresAt = DateTime.UtcNow.AddHours(24)
                }
            };

            return ServiceResult<AuthLoginResponse?>.Ok(authData, null);
        }
        catch (Exception ex)
        {
            return ServiceResult<AuthLoginResponse?>.Fail(null, ex.Message);
        }
    }

    public async Task<ServiceResult<object?>> RegisterAsync(AuthRegisterRequest request)
    {
        try
        {
            _logger.LogInformation("Registering user with email: {email}", request.Email);

            User? existingUser = await GetUserByEmailAsync(request.Email);

            if (existingUser is not null)
            {
                _logger.LogInformation("User with email {email} already exists.", request.Email);
                return ServiceResult<object?>.Fail(null, "User with this email already exists.");
            }

            User user = new()
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Email = request.Email.ToLower(),
                PasswordHash = HashPassword(request.Password),
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
            };

            if (string.IsNullOrWhiteSpace(user.Email))
            {
                _logger.LogWarning("Email is missing or empty.");
                return ServiceResult<object?>.Fail(null, "Email is missing or empty.");
            }

            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            try
            {
                _logger.LogInformation("Sending welcome email to: {email}", user.Email);

                var payLoad = new
                {
                    UserName = user.Name
                };

                bool emailSent = await _emailService.SendTemplateEmailAsync(
                    to: user.Email,
                    templateId: 40590712,
                    templateModel: payLoad
                );

                if (!emailSent)
                {
                    _logger.LogWarning("Failed to send welcome email.");
                    return ServiceResult<object?>.Ok(null, "Registration successful.", ["Failed to send welcome email."]);
                }

                _logger.LogInformation("User registered successfully: {email}, and welcome email has been sent", request.Email);
            }
            catch (Exception emailEx)
            {
                _logger.LogError("Failed to send welcome email: {message}", emailEx.Message);
                return ServiceResult<object?>.Ok(null, "Registration successful.", ["Failed to send welcome email."]);
            }

            _logger.LogInformation("User registered successfully: {email}, and welcome email has been sent", request.Email);
            return ServiceResult<object?>.Ok(null, "Registration successful.", ["Failed to send welcome email."]);
        }
        catch (Exception ex)
        {
            string errorMessage = ex.InnerException?.Message ?? ex.Message;
            _logger.LogError("Failed to register user: {message}", errorMessage);
            return ServiceResult<object?>.Fail(null, "Failed to register user", [errorMessage]);
        }
    }

    public async Task<UserProfileResponse?> GetUserProfileByIdAsync(Guid userId)
    {
        User? user = await _dbContext.Users.FindAsync(userId);

        return user is null
            ? null
            : new UserProfileResponse
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                CreatedAt = user.CreatedAt,
                LastLoginAt = user.LastLoginAt
            };
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
    }

    public async Task<ServiceResult<object?>> LogoutAsync(Guid userId)
    {
        User? user = await _dbContext.Users.FindAsync(userId);

        if (user is null) return ServiceResult<object?>.Fail(null, "User not found.");

        user.LastLogoutAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();

        try
        {
            var model = new
            {
                UserName = user.Name,
                LogoutTime = user.LastLogoutAt?.ToString("f") ?? "unknown"
            };

            await _emailService.SendTemplateEmailAsync(
                to: user.Email,
                templateId: 40597431,
                templateModel: model
            );
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Failed to send logout email: {message}", ex.Message);
        }

        return ServiceResult<object?>.Ok(null, "Logout successful.");
    }

    public string GenerateJwtToken(User user)
    {
        IConfigurationSection? jwtSettings = _configuration.GetSection("JwtSettings") ?? throw new InvalidOperationException("JWT settings not configured");

        string secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT secret key not configured");
        string issuer = jwtSettings["Issuer"] ?? "ExpenseTracker";
        string audience = jwtSettings["Audience"] ?? "ExpenseTracker";

        SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(secretKey));
        SigningCredentials credentials = new(key, SecurityAlgorithms.HmacSha256);

        Claim[] claims =
        [
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Name),
            new(ClaimTypes.Email, user.Email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
        ];

        JwtSecurityToken token = new(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(24),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static bool VerifyPassword(string password, string hash)
    {
        return Encryption.BCrypt.Verify(password, hash);
    }

    public static string HashPassword(string password)
    {
        return Encryption.BCrypt.HashPassword(password);
    }
}
