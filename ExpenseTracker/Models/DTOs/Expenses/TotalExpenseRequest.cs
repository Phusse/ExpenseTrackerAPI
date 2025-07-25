namespace ExpenseTracker.Models.DTOs.Expenses;

/// <summary>
/// Request details for calculating total expenses within optional date-based filters.
/// </summary>
public class TotalExpenseRequest
{
	/// <summary>
	/// The start date for filtering expenses. Optional.
	/// </summary>
	public DateOnly? StartDate { get; set; }

	/// <summary>
	/// The end date for filtering expenses. Optional.
	/// </summary>
	public DateOnly? EndDate { get; set; }

	/// <summary>
	/// The specific period to filter by, typically the first day of the month (e.g., 2025-07-01 for July 2025).
	/// </summary>
	public DateOnly? Period { get; set; }
}
