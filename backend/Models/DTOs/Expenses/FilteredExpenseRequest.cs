using ExpenseTracker.Enums;

namespace ExpenseTracker.Models.DTOs.Expenses;

/// <summary>
/// Represents filter options for querying a user's expenses by date, amount, or category.
/// </summary>
public class FilteredExpenseRequest
{
	/// <summary>
	/// The start date to filter expenses from. Optional.
	/// </summary>
	public DateTime? StartDate { get; set; }

	/// <summary>
	/// The end date to filter expenses up to. Optional.
	/// </summary>
	public DateTime? EndDate { get; set; }

	/// <summary>
	/// The minimum amount to include in the filtered results. Optional.
	/// </summary>
	public decimal? MinAmount { get; set; }

	/// <summary>
	/// The maximum amount to include in the filtered results. Optional.
	/// </summary>
	public decimal? MaxAmount { get; set; }

	/// <summary>
	/// A specific amount to match exactly. Optional.
	/// </summary>
	public decimal? ExactAmount { get; set; }

	/// <summary>
	/// The expense category to filter by (e.g., Food, Utilities, etc.). Optional.
	/// </summary>
	public ExpenseCategory? Category { get; set; }
}
