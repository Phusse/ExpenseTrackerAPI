using ExpenseTracker.Enums;

namespace ExpenseTracker.Models.DTOs.Budget;

/// <summary>
/// Represents a summary of a user's budget usage for a specific category and time period.
/// </summary>
public class BudgetSummaryResponse
{
    /// <summary>
    /// The expense category this summary is for.
    /// </summary>
    public required ExpenseCategory Category { get; set; }

    /// <summary>
    /// The period (month and year) this budget summary represents.
    /// </summary>
    public required DateOnly Period { get; set; }

    /// <summary>
    /// The total budgeted amount for this category and period.
    /// </summary>
    public required double BudgetedAmount { get; set; }

    /// <summary>
    /// The amount already spent in this category and period.
    /// </summary>
    public required double SpentAmount { get; set; }

    /// <summary>
    /// The percentage of the budget that has been used (0 to 100).
    /// </summary>
    public required double PercentageUsed { get; set; }

    /// <summary>
    /// A message giving context or warnings about the budget usage.
    /// </summary>
    public required string Message { get; set; }
}
