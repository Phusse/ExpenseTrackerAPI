namespace ExpenseTracker.Models.DTOs.Expenses;

/// <summary>
/// Request details for calculating total expenses within optional filters.
/// </summary>
public class TotalExpenseRequest
{
	public DateTime? StartDate { get; set; }
	public DateTime? EndDate { get; set; }
	public int? Month { get; set; }
	public int? Year { get; set; }
}
