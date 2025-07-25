namespace ExpenseTracker.Models.DTOs.Expenses;

/// <summary>
/// Represents filter options when querying user expenses.
/// </summary>
public class FilteredExpenseRequest
{
	public DateTime? StartDate { get; set; }
	public DateTime? EndDate { get; set; }
	public decimal? MinAmount { get; set; }
	public decimal? MaxAmount { get; set; }
	public decimal? ExactAmount { get; set; }
	public string? Category { get; set; }
}
