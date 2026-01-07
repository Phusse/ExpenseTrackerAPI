namespace ExpenseTracker.Models.DTOs.Settings;

/// <summary>
/// Request DTO for updating user profile
/// </summary>
public class UpdateProfileRequest
{
    public string? Name { get; set; }
    public string? Email { get; set; }
}

/// <summary>
/// Request DTO for changing password
/// </summary>
public class ChangePasswordRequest
{
    public required string CurrentPassword { get; set; }
    public required string NewPassword { get; set; }
    public required string ConfirmNewPassword { get; set; }
}

/// <summary>
/// Request DTO for updating user settings/preferences
/// </summary>
public class UpdateSettingsRequest
{
    public bool? NotificationsEnabled { get; set; }
    public bool? DarkModeEnabled { get; set; }
    public string? Currency { get; set; }
    public string? Locale { get; set; }
    public int? BudgetReminderDay { get; set; }
    public bool? WeeklyReportEnabled { get; set; }
}

/// <summary>
/// Response DTO for user settings
/// </summary>
public class UserSettingsResponse
{
    public bool NotificationsEnabled { get; set; }
    public bool DarkModeEnabled { get; set; }
    public string Currency { get; set; } = "NGN";
    public string Locale { get; set; } = "en-US";
    public int BudgetReminderDay { get; set; }
    public bool WeeklyReportEnabled { get; set; }
}

/// <summary>
/// Response DTO for data export
/// </summary>
public class DataExportResponse
{
    public required string DownloadUrl { get; set; }
    public required string FileName { get; set; }
    public DateTime ExpiresAt { get; set; }
}



/// <summary>
/// Supported currencies
/// </summary>
public static class SupportedCurrencies
{
    public static readonly Dictionary<string, string> Currencies = new()
    {
        { "NGN", "Nigerian Naira (₦)" },
        { "USD", "US Dollar ($)" },
        { "EUR", "Euro (€)" },
        { "GBP", "British Pound (£)" },
        { "GHS", "Ghanaian Cedi (₵)" },
        { "KES", "Kenyan Shilling (KSh)" },
        { "ZAR", "South African Rand (R)" }
    };
}
