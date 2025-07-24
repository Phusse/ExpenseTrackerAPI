using System.ComponentModel.DataAnnotations;
using ExpenseTracker.Enums;

namespace ExpenseTracker.Models.DTOs.Budgets;

/// <summary>
/// Represents a request to retrieve the budget status for a specific category and time period.
/// </summary>
public class BudgetStatusRequest
{
	/// <summary>
	/// The category of expense to retrieve the budget status for.
	/// </summary>
	/// <remarks>This field is required.</remarks>
	[Required(ErrorMessage = "Category is not set.")]
	public ExpenseCategory? Category { get; set; }

	/// <summary>
	/// The month and year for which the budget status is being requested.
	/// </summary>
	/// <remarks>This field is required.</remarks>
	[Required(ErrorMessage = "Period is not set.")]
	public DateOnly? Period { get; set; }
}
