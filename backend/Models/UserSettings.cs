using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseTracker.Models;

/// <summary>
/// Represents user preferences and settings
/// </summary>
public class UserSettings
{
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// Foreign key to the User
    /// </summary>
    [Required]
    public Guid UserId { get; set; }

    /// <summary>
    /// Whether push/email notifications are enabled
    /// </summary>
    public bool NotificationsEnabled { get; set; } = true;

    /// <summary>
    /// Whether dark mode is enabled
    /// </summary>
    public bool DarkModeEnabled { get; set; } = true;

    /// <summary>
    /// Preferred currency code (e.g., NGN, USD, EUR)
    /// </summary>
    [StringLength(3)]
    public string Currency { get; set; } = "NGN";

    /// <summary>
    /// User's preferred language/locale
    /// </summary>
    [StringLength(10)]
    public string Locale { get; set; } = "en-US";

    /// <summary>
    /// Monthly budget reminder day (1-28)
    /// </summary>
    public int BudgetReminderDay { get; set; } = 1;

    /// <summary>
    /// Whether weekly spending reports are enabled
    /// </summary>
    public bool WeeklyReportEnabled { get; set; } = true;

    /// <summary>
    /// Date the settings were created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Date the settings were last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Navigation property to User
    /// </summary>
    [ForeignKey("UserId")]
    public virtual User? User { get; set; }
}
