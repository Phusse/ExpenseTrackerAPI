using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ExpenseTracker.Data;
using ExpenseTracker.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
// using ExpenseTracker.Services.EmailService;
using ExpenseTracker.Services;
using BCrypt.Net;

namespace ExpenseTracker.Services;

public class AuthService : IAuthService
{
    private readonly ExpenseTrackerDbContext _dbContext;
    private readonly IConfiguration _configuration;
    private readonly IEmailService _emailService;

    public AuthService(ExpenseTrackerDbContext dbContext, IConfiguration configuration, IEmailService emailService)
    {
        _dbContext = dbContext;
        _configuration = configuration;
        _emailService = emailService;
    }

    public async Task<(bool IsSuccess, AuthData? Data, string? ErrorMessage)> LoginAsync(LoginRequest request)
    {
        try
        {
            var user = await GetUserByEmailAsync(request.Email);

            if (user == null || !VerifyPassword(request.Password, user.PasswordHash))
            {
                return (false, null, "Invalid email or password");
            }

            if (!user.IsActive)
            {
                return (false, null, "Account is deactivated");
            }

            user.LastLoginAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();
            try
            {
                var model = new
                {
                    UserName = user.Name,
                    LoginTime = user.LastLoginAt?.ToString("f"),
                    CurrentYear = DateTime.Now.Year
                };

                await _emailService.SendTemplateEmailAsync(
                    to: user.Email,
                    templateId: 40597432, 
                    templateModel: model
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[WARN] Login email failed: {ex.Message}");
            }

            var token = GenerateJwtToken(user);
            var authData = new AuthData
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddHours(24)
            };

            return (true, authData, null);
        }
        catch (Exception ex)
        {
            return (false, null, ex.Message);
        }
    }

    public async Task<(bool IsSuccess, User? Data, string? ErrorMessage)> RegisterAsync(RegisterRequest request)
    {
        try
        {
            Console.WriteLine($"[INFO] Registering user with email: {request.Email}");

            var existingUser = await GetUserByEmailAsync(request.Email);
            if (existingUser != null)
            {
                Console.WriteLine($"[INFO] User with email {request.Email} already exists.");
                return (false, null, "User with this email already exists");
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Email = request.Email.ToLower(),
                PasswordHash = HashPassword(request.Password),
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            if (string.IsNullOrWhiteSpace(user.Email))
            {
                Console.WriteLine("[WARNING] Email is missing or empty.");
                return (false, null, "Email address is missing or empty.");
            }

            try
            {
                Console.WriteLine($"[INFO] Attempting to send welcome email to: {user.Email}");

                var model = new
                {
                    UserName = user.Name
                };
                var emailSent = await _emailService.SendTemplateEmailAsync(
                    to: user.Email,
                    templateId: 40590712,
                    templateModel: model
                );

                if (!emailSent)
                {
                    Console.WriteLine("[WARN] Email service returned false (not sent).");
                    return (false, null, "Registration saved, but failed to send welcome email.");
                }

                Console.WriteLine("[INFO] Welcome email sent successfully.");
            }
            catch (Exception emailEx)
            {
                Console.WriteLine($"[ERROR] Failed to send welcome email: {emailEx}");
                return (false, null, $"Registration saved but failed to send email: {emailEx.Message}");
            }

            Console.WriteLine($"[SUCCESS] User registered successfully: {user.Email}");
            return (true, user, null);
        }
        catch (Exception ex)
        {
            var errorMessage = ex.InnerException?.Message ?? ex.Message;
            Console.WriteLine($"[ERROR] Registration failed for {request.Email}: {errorMessage}");
            return (false, null, $"Registration failed: {errorMessage}");
        }
    }


    public async Task<User?> GetUserByIdAsync(Guid userId)
    {
        return await _dbContext.Users.FindAsync(userId);
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
    }
    public async Task<(bool IsSuccess, string? Message)> LogoutAsync(Guid userId)
    {
        var user = await _dbContext.Users.FindAsync(userId);
        if (user == null)
            return (false, "User not found");

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
            Console.WriteLine($"[WARN] Failed to send logout email: {ex.Message}");
        }

        return (true, "Logout successful");
    }

    public string GenerateJwtToken(User user)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not configured");
        var issuer = jwtSettings["Issuer"] ?? "ExpenseTracker";
        var audience = jwtSettings["Audience"] ?? "ExpenseTracker";

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(24),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public bool VerifyPassword(string password, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(password, hash);
    }

    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }
}