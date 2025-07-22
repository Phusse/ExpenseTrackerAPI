using System.ComponentModel.DataAnnotations;
using ExpenseTracker.Enums;

namespace ExpenseTracker.Models;

/// <summary>
/// Represents a monthly budget allocation for a specific expense category and user.
/// </summary>
public class Budget
{
    /// <summary>
    /// Primary key identifier for the budget entry.
    /// </summary>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// The ID of the user this budget belongs to.
    /// </summary>
    [Required]
    public Guid UserId { get; set; }

    /// <summary>
    /// The category of expenses this budget applies to.
    /// </summary>
    [Required]
    public ExpenseCategory Category { get; set; }

    /// <summary>
    /// The maximum amount allowed for this category in the given month.
    /// </summary>
    [Required]
    public double Limit { get; set; }

    /// <summary>
    /// The date representing the month and year for the budget.
    /// Typically use the first day of the month (e.g., 2025-07-01).
    /// </summary>
    [Required]
    public DateOnly Period { get; set; }

    /// <summary>
    /// The UTC timestamp when the budget was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Navigation property to the associated user.
    /// </summary>
    public User? User { get; set; }
}
