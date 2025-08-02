using ExpenseTracker.Enums;

namespace ExpenseTracker.Models.DTOs.Budgets;

/// <summary>
/// Represents a summary of a user's budget usage for a specific category and time period.
/// Includes calculated metrics like percentage of budget used and contextual usage messages.
/// </summary>
public class BudgetSummaryResponse
{
    /// <summary>
    /// The unique identifier of the budget summary.
    /// </summary>
    public required Guid? Id { get; set; }

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
    /// Gets the remaining amount in the budget (Budgeted - Spent).
    /// </summary>
    public double RemainingAmount => BudgetedAmount - SpentAmount;

    /// <summary>
    /// The percentage of the budget that has been used, calculated as (SpentAmount / BudgetedAmount) * 100.
    /// Returns 0 if the budgeted amount is zero.
    /// </summary>
    public double PercentageUsed => BudgetedAmount == 0
        ? 0
        : Math.Round(SpentAmount / BudgetedAmount * 100, 2);

    /// <summary>
    /// A context-sensitive message indicating the user's current budget usage status:
    /// - "No budget set for this category." if BudgetedAmount is 0.
    /// - "You have exceeded your budget!" if usage exceeds 100%.
    /// - "You have reached your budget limit." if usage is exactly 100%.
    /// - "You have used more than 70% of your budget." if usage is 70% or higher.
    /// - "You are within your budget." otherwise.
    /// </summary>
    public string Message => BudgetedAmount switch
    {
        0 => "No budget set for this category.",
        _ => PercentageUsed switch
        {
            > 100 => "You have exceeded your budget!",
            100 => "You have reached your budget limit.",
            >= 70 => "You have used more than 70% of your budget.",
            _ => "You are within your budget.",
        }
    };
}
