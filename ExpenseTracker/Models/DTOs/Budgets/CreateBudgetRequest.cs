using System.ComponentModel.DataAnnotations;
using ExpenseTracker.Enums;

namespace ExpenseTracker.Models.DTOs.Budgets;

/// <summary>
/// Request model used for creating a new budget entry.
/// </summary>
public class CreateBudgetRequest
{
    /// <summary>
    /// The expense category the budget applies to.
    /// </summary>
    [Required]
    public ExpenseCategory Category { get; set; }

    /// <summary>
    /// The spending limit for the selected category during the specified period.
    /// Must be greater than 0.
    /// </summary>
    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Limit must be greater than 0.")]
    public double Limit { get; set; }

    /// <summary>
    /// The period (month and year) the budget applies to.
    /// Only the month and year components are used; the day should typically be the first of the month.
    /// </summary>
    [Required]
    public DateOnly Period { get; set; }
}
