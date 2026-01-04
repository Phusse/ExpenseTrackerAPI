namespace ExpenseTracker.Models.DTOs.Budgets;

/// <summary>
/// Represents a budget item returned in budget listings.
/// </summary>
public class BudgetListItemDto
{
    /// <summary>
    /// The unique identifier of the budget.
    /// </summary>
    public required Guid Id { get; set; }

    /// <summary>
    /// The expense category for this budget.
    /// </summary>
    public required string Category { get; set; }

    /// <summary>
    /// The budget limit amount.
    /// </summary>
    public required double Limit { get; set; }

    /// <summary>
    /// The amount spent so far in this budget period.
    /// </summary>
    public required double Spent { get; set; }

    /// <summary>
    /// The period (month and year) this budget applies to.
    /// </summary>
    public required DateOnly Period { get; set; }
}
