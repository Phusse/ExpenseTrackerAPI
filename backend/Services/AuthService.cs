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

            _ = Task.Run(async () =>
            {
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
                    _logger.LogWarning("Failed to send login notification email: {ex.Message}", ex.Message);
                }
            });

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

            _ = Task.Run(async () =>
            {
                try
                {
                    _logger.LogInformation("Sending welcome email to: {email}", user.Email);

                    var payLoad = new
                    {
                        UserName = user.Name
                    };

                    await _emailService.SendTemplateEmailAsync(
                        to: user.Email,
                        templateId: 40590712,
                        templateModel: payLoad
                    );
                    _logger.LogInformation("Welcome email sent to: {email}", user.Email);
                }
                catch (Exception emailEx)
                {
                    _logger.LogError("Failed to send welcome email: {message}", emailEx.Message);
                }
            });

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

        _ = Task.Run(async () =>
        {
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
        });

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

    public async Task<ServiceResult<UserProfileResponse>> UpdateProfileAsync(Guid userId, string? name, string? email)
    {
        try
        {
            var user = await _dbContext.Users.FindAsync(userId);
            if (user == null)
            {
                return ServiceResult<UserProfileResponse>.Fail(null!, "User not found.");
            }

            // Update name if provided
            if (!string.IsNullOrWhiteSpace(name))
            {
                user.Name = name.Trim();
            }

            // Update email if provided and different
            if (!string.IsNullOrWhiteSpace(email) && email.ToLower() != user.Email.ToLower())
            {
                // Check if email is already in use
                var existingUser = await GetUserByEmailAsync(email);
                if (existingUser != null)
                {
                    return ServiceResult<UserProfileResponse>.Fail(null!, "Email is already in use by another account.");
                }
                user.Email = email.ToLower().Trim();
            }

            await _dbContext.SaveChangesAsync();

            return ServiceResult<UserProfileResponse>.Ok(new UserProfileResponse
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                CreatedAt = user.CreatedAt,
                LastLoginAt = user.LastLoginAt
            }, "Profile updated successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to update profile: {message}", ex.Message);
            return ServiceResult<UserProfileResponse>.Fail(null!, "Failed to update profile.");
        }
    }

    public async Task<ServiceResult<object?>> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword)
    {
        try
        {
            var user = await _dbContext.Users.FindAsync(userId);
            if (user == null)
            {
                return ServiceResult<object?>.Fail(null, "User not found.");
            }

            // Verify current password
            if (!VerifyPassword(currentPassword, user.PasswordHash))
            {
                return ServiceResult<object?>.Fail(null, "Current password is incorrect.");
            }

            // Validate new password
            if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 6)
            {
                return ServiceResult<object?>.Fail(null, "New password must be at least 6 characters.");
            }

            // Update password
            user.PasswordHash = HashPassword(newPassword);
            await _dbContext.SaveChangesAsync();

            // Send password change notification email
            // Send password change notification email (fire-and-forget)
            _ = Task.Run(async () =>
            {
                try
                {
                    var model = new
                    {
                        UserName = user.Name,
                        ChangeTime = DateTime.UtcNow.ToString("f")
                    };

                    await _emailService.SendTemplateEmailAsync(
                        to: user.Email,
                        templateId: 40597432, // Use appropriate template
                        templateModel: model
                    );
                }
                catch (Exception emailEx)
                {
                    _logger.LogWarning("Failed to send password change email: {message}", emailEx.Message);
                }
            });

            return ServiceResult<object?>.Ok(null, "Password changed successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to change password: {message}", ex.Message);
            return ServiceResult<object?>.Fail(null, "Failed to change password.");
        }
    }

    public async Task<ServiceResult<object?>> RegisterWithSecurityQuestionsAsync(AuthRegisterWithSecurityRequest request)
    {
        try
        {
            // Validate security questions
            if (request.SecurityQuestions == null || request.SecurityQuestions.Count != 3)
            {
                return ServiceResult<object?>.Fail(null, "Exactly 3 security questions are required.");
            }

            var questionIds = request.SecurityQuestions.Select(q => q.QuestionId).ToList();
            if (questionIds.Distinct().Count() != 3)
            {
                return ServiceResult<object?>.Fail(null, "All 3 security questions must be different.");
            }

            foreach (var q in request.SecurityQuestions)
            {
                if (!SecurityQuestions.Questions.ContainsKey(q.QuestionId))
                {
                    return ServiceResult<object?>.Fail(null, $"Invalid question ID: {q.QuestionId}");
                }
                if (string.IsNullOrWhiteSpace(q.Answer) || q.Answer.Trim().Length < 2)
                {
                    return ServiceResult<object?>.Fail(null, "Security answer must be at least 2 characters.");
                }
            }

            // Check existing user
            User? existingUser = await GetUserByEmailAsync(request.Email);
            if (existingUser is not null)
            {
                return ServiceResult<object?>.Fail(null, "User with this email already exists.");
            }

            // Create user
            User user = new()
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Email = request.Email.ToLower(),
                PasswordHash = HashPassword(request.Password),
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
            };

            await _dbContext.Users.AddAsync(user);

            // Create security questions
            int order = 1;
            foreach (var sq in request.SecurityQuestions)
            {
                var securityQuestion = new SecurityQuestion
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    QuestionId = sq.QuestionId,
                    AnswerHash = HashSecurityAnswer(sq.Answer),
                    QuestionOrder = order++,
                    CreatedAt = DateTime.UtcNow
                };
                await _dbContext.SecurityQuestions.AddAsync(securityQuestion);
            }

            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("User registered with security questions: {email}", request.Email);
            return ServiceResult<object?>.Ok(null, "Registration successful.");
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to register user with security questions: {message}", ex.Message);
            return ServiceResult<object?>.Fail(null, "Failed to register user.");
        }
    }

    public async Task<ServiceResult<ForgotPasswordQuestionsResponse>> GetSecurityQuestionsForResetAsync(string email)
    {
        try
        {
            var user = await GetUserByEmailAsync(email);
            if (user == null)
            {
                return ServiceResult<ForgotPasswordQuestionsResponse>.Fail(null!, "No account found with this email.");
            }

            var questions = await _dbContext.SecurityQuestions
                .Where(sq => sq.UserId == user.Id)
                .OrderBy(sq => sq.QuestionOrder)
                .ToListAsync();

            if (questions.Count == 0)
            {
                return ServiceResult<ForgotPasswordQuestionsResponse>.Fail(null!, "No security questions set for this account.");
            }

            var response = new ForgotPasswordQuestionsResponse
            {
                Email = email,
                Questions = questions.Select(q => new UserSecurityQuestion
                {
                    QuestionOrder = q.QuestionOrder,
                    QuestionId = q.QuestionId,
                    Question = SecurityQuestions.GetQuestion(q.QuestionId)
                }).ToList()
            };

            return ServiceResult<ForgotPasswordQuestionsResponse>.Ok(response, "Security questions retrieved.");
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to get security questions: {message}", ex.Message);
            return ServiceResult<ForgotPasswordQuestionsResponse>.Fail(null!, "Failed to retrieve security questions.");
        }
    }

    public async Task<ServiceResult<List<UserSecurityQuestion>>> GetMySecurityQuestionsAsync(Guid userId)
    {
        try
        {
            var questions = await _dbContext.SecurityQuestions
                .Where(sq => sq.UserId == userId)
                .OrderBy(sq => sq.QuestionOrder)
                .ToListAsync();

            if (questions.Count == 0)
            {
                return ServiceResult<List<UserSecurityQuestion>>.Fail(null!, "No security questions set for this account.");
            }

            var result = questions.Select(q => new UserSecurityQuestion
            {
                QuestionOrder = q.QuestionOrder,
                QuestionId = q.QuestionId,
                Question = SecurityQuestions.GetQuestion(q.QuestionId)
            }).ToList();

            return ServiceResult<List<UserSecurityQuestion>>.Ok(result, "Security questions retrieved.");
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to get security questions: {message}", ex.Message);
            return ServiceResult<List<UserSecurityQuestion>>.Fail(null!, "Failed to retrieve security questions.");
        }
    }

    public async Task<ServiceResult<object?>> ResetPasswordWithSecurityQuestionsAsync(ResetPasswordRequest request)
    {
        try
        {
            if (request.NewPassword != request.ConfirmNewPassword)
            {
                return ServiceResult<object?>.Fail(null, "Passwords do not match.");
            }

            if (string.IsNullOrWhiteSpace(request.NewPassword) || request.NewPassword.Length < 6)
            {
                return ServiceResult<object?>.Fail(null, "Password must be at least 6 characters.");
            }

            var user = await GetUserByEmailAsync(request.Email);
            if (user == null)
            {
                return ServiceResult<object?>.Fail(null, "No account found with this email.");
            }

            var storedQuestions = await _dbContext.SecurityQuestions
                .Where(sq => sq.UserId == user.Id)
                .ToListAsync();

            if (storedQuestions.Count == 0)
            {
                return ServiceResult<object?>.Fail(null, "No security questions set for this account.");
            }

            // Verify all answers
            foreach (var answer in request.Answers)
            {
                var storedQ = storedQuestions.FirstOrDefault(q => q.QuestionId == answer.QuestionId);
                if (storedQ == null)
                {
                    return ServiceResult<object?>.Fail(null, "Invalid security question.");
                }

                if (!VerifySecurityAnswer(answer.Answer, storedQ.AnswerHash))
                {
                    return ServiceResult<object?>.Fail(null, "One or more security answers are incorrect.");
                }
            }

            // All answers correct - reset password
            user.PasswordHash = HashPassword(request.NewPassword);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Password reset via security questions for: {email}", request.Email);
            return ServiceResult<object?>.Ok(null, "Password reset successfully. You can now log in.");
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to reset password: {message}", ex.Message);
            return ServiceResult<object?>.Fail(null, "Failed to reset password.");
        }
    }

    public SecurityQuestionsListResponse GetAvailableSecurityQuestions()
    {
        return new SecurityQuestionsListResponse
        {
            Questions = SecurityQuestions.Questions.Select(q => new SecurityQuestionItem
            {
                Id = q.Key,
                Question = q.Value
            }).ToList()
        };
    }

    private static string HashSecurityAnswer(string answer)
    {
        // Normalize: lowercase, trim whitespace
        var normalized = answer.Trim().ToLowerInvariant();
        return Encryption.BCrypt.HashPassword(normalized);
    }

    private static bool VerifySecurityAnswer(string answer, string hash)
    {
        var normalized = answer.Trim().ToLowerInvariant();
        return Encryption.BCrypt.Verify(normalized, hash);
    }
}
