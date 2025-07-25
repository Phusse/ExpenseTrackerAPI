namespace ExpenseTracker.Models.DTOs.Expenses;

/// <summary>
/// Request details for calculating total expenses within optional date-based filters.
/// </summary>
public class TotalExpenseRequest
{
	/// <summary>
	/// The start date for filtering expenses. Optional.
	/// </summary>
	public DateTime? StartDate { get; set; }

	/// <summary>
	/// The end date for filtering expenses. Optional.
	/// </summary>
	public DateTime? EndDate { get; set; }

	/// <summary>
	/// The specific month to filter by (1–12). Optional.
	/// </summary>
	public int? Month { get; set; }

	/// <summary>
	/// The specific year to filter by (e.g., 2025). Optional.
	/// </summary>
	public int? Year { get; set; }
}
