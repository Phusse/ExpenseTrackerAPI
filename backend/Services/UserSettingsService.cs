using ExpenseTracker.Data;
using ExpenseTracker.Models;
using ExpenseTracker.Models.DTOs.Settings;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace ExpenseTracker.Services;

/// <summary>
/// Service for managing user settings and preferences
/// </summary>
public class UserSettingsService(ExpenseTrackerDbContext context)
{
    private readonly ExpenseTrackerDbContext _context = context;

    /// <summary>
    /// Get user settings, creating defaults if not exists
    /// </summary>
    public async Task<UserSettingsResponse> GetSettingsAsync(Guid userId)
    {
        var settings = await _context.UserSettings
            .FirstOrDefaultAsync(s => s.UserId == userId);

        if (settings == null)
        {
            // Create default settings
            settings = new UserSettings
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                NotificationsEnabled = true,
                DarkModeEnabled = true,
                Currency = "NGN",
                Locale = "en-US",
                BudgetReminderDay = 1,
                WeeklyReportEnabled = true
            };
            _context.UserSettings.Add(settings);
            await _context.SaveChangesAsync();
        }

        return new UserSettingsResponse
        {
            NotificationsEnabled = settings.NotificationsEnabled,
            DarkModeEnabled = settings.DarkModeEnabled,
            Currency = settings.Currency,
            Locale = settings.Locale,
            BudgetReminderDay = settings.BudgetReminderDay,
            WeeklyReportEnabled = settings.WeeklyReportEnabled
        };
    }

    /// <summary>
    /// Update user settings
    /// </summary>
    public async Task<ServiceResult<UserSettingsResponse>> UpdateSettingsAsync(Guid userId, UpdateSettingsRequest request)
    {
        var settings = await _context.UserSettings
            .FirstOrDefaultAsync(s => s.UserId == userId);

        if (settings == null)
        {
            // Create new settings
            settings = new UserSettings
            {
                Id = Guid.NewGuid(),
                UserId = userId
            };
            _context.UserSettings.Add(settings);
        }

        // Update only provided fields
        if (request.NotificationsEnabled.HasValue)
            settings.NotificationsEnabled = request.NotificationsEnabled.Value;
        if (request.DarkModeEnabled.HasValue)
            settings.DarkModeEnabled = request.DarkModeEnabled.Value;
        if (!string.IsNullOrEmpty(request.Currency))
            settings.Currency = request.Currency;
        if (!string.IsNullOrEmpty(request.Locale))
            settings.Locale = request.Locale;
        if (request.BudgetReminderDay.HasValue && request.BudgetReminderDay >= 1 && request.BudgetReminderDay <= 28)
            settings.BudgetReminderDay = request.BudgetReminderDay.Value;
        if (request.WeeklyReportEnabled.HasValue)
            settings.WeeklyReportEnabled = request.WeeklyReportEnabled.Value;

        settings.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return ServiceResult<UserSettingsResponse>.Ok(new UserSettingsResponse
        {
            NotificationsEnabled = settings.NotificationsEnabled,
            DarkModeEnabled = settings.DarkModeEnabled,
            Currency = settings.Currency,
            Locale = settings.Locale,
            BudgetReminderDay = settings.BudgetReminderDay,
            WeeklyReportEnabled = settings.WeeklyReportEnabled
        }, "Settings updated successfully.");
    }

    /// <summary>
    /// Export all user data as JSON
    /// </summary>
    public async Task<string> ExportUserDataAsync(Guid userId)
    {
        var user = await _context.Users.FindAsync(userId);
        var expenses = await _context.Expenses.Where(e => e.UserId == userId).ToListAsync();
        var budgets = await _context.Budgets.Where(b => b.UserId == userId).ToListAsync();
        var goals = await _context.SavingGoals.Where(g => g.UserId == userId).ToListAsync();
        var incomes = await _context.Incomes.Where(i => i.UserId == userId).ToListAsync();
        var settings = await _context.UserSettings.FirstOrDefaultAsync(s => s.UserId == userId);

        var exportData = new
        {
            ExportedAt = DateTime.UtcNow,
            User = new
            {
                user?.Name,
                user?.Email,
                user?.CreatedAt
            },
            Settings = settings != null ? new
            {
                settings.NotificationsEnabled,
                settings.DarkModeEnabled,
                settings.Currency,
                settings.Locale
            } : null,
            Expenses = expenses.Select(e => new
            {
                e.Amount,
                e.Description,
                Category = e.Category.ToString(),
                PaymentMethod = e.PaymentMethod.ToString(),
                e.DateOfExpense,
                e.DateRecorded
            }),
            Budgets = budgets.Select(b => new
            {
                b.Limit,
                Category = b.Category.ToString(),
                b.Period,
                b.CreatedAt
            }),
            SavingGoals = goals.Select(g => new
            {
                g.Title,
                g.Description,
                g.TargetAmount,
                g.CurrentAmount,
                g.Deadline,
                Status = g.Status.ToString()
            }),
            Incomes = incomes.Select(i => new
            {
                i.Amount,
                i.Description,
                Source = i.Source.ToString(),
                Frequency = i.Frequency?.ToString(),
                i.DateReceived
            })
        };

        return JsonSerializer.Serialize(exportData, new JsonSerializerOptions
        {
            WriteIndented = true
        });
    }

    /// <summary>
    /// Delete user account and all associated data
    /// </summary>
    /// <summary>
    /// Delete user account and all associated data
    /// </summary>
    public async Task<ServiceResult<object?>> DeleteAccountAsync(Guid userId, string password, int? questionId, string? answer)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            return ServiceResult<object?>.Fail(null, "User not found.");
        }

        // Verify password
        if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
        {
            return ServiceResult<object?>.Fail(null, "Incorrect password.");
        }

        // Verify Security Question (if provided)
        if (questionId.HasValue && !string.IsNullOrWhiteSpace(answer))
        {
            var securityQuestion = await _context.SecurityQuestions
                .FirstOrDefaultAsync(sq => sq.UserId == userId && sq.QuestionId == questionId.Value);

            if (securityQuestion == null)
            {
                 // Failsafe: if user has questions but passed wrong ID
                 // Or if user selected a question they don't have set
                 return ServiceResult<object?>.Fail(null, "Security question not found for this account.");
            }

            var normalizedAnswer = answer.Trim().ToLowerInvariant();
            if (!BCrypt.Net.BCrypt.Verify(normalizedAnswer, securityQuestion.AnswerHash))
            {
                return ServiceResult<object?>.Fail(null, "Incorrect security answer.");
            }
        }
        else
        {
             // Enforce security question if user has them
             var hasQuestions = await _context.SecurityQuestions.AnyAsync(sq => sq.UserId == userId);
             if (hasQuestions)
             {
                 return ServiceResult<object?>.Fail(null, "Security question verification is required.");
             }
        }

        // Delete all user data (cascade will handle most, but let's be explicit)
        var settings = await _context.UserSettings.FirstOrDefaultAsync(s => s.UserId == userId);
        if (settings != null) _context.UserSettings.Remove(settings);

        var contributions = await _context.SavingGoalContributions
            .Include(c => c.SavingGoal)
            .Where(c => c.SavingGoal!.UserId == userId)
            .ToListAsync();
        _context.SavingGoalContributions.RemoveRange(contributions);

        var goals = await _context.SavingGoals.Where(g => g.UserId == userId).ToListAsync();
        _context.SavingGoals.RemoveRange(goals);

        var budgets = await _context.Budgets.Where(b => b.UserId == userId).ToListAsync();
        _context.Budgets.RemoveRange(budgets);

        var incomes = await _context.Incomes.Where(i => i.UserId == userId).ToListAsync();
        _context.Incomes.RemoveRange(incomes);

        var expenses = await _context.Expenses.Where(e => e.UserId == userId).ToListAsync();
        _context.Expenses.RemoveRange(expenses);
        
        var userQuestions = await _context.SecurityQuestions.Where(sq => sq.UserId == userId).ToListAsync();
        _context.SecurityQuestions.RemoveRange(userQuestions);

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        return ServiceResult<object?>.Ok(null, "Account deleted successfully.");
    }
}
