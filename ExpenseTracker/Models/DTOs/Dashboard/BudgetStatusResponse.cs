using ExpenseTracker.Enums;

namespace ExpenseTracker.Models.DTOs.Dashboard;

/// <summary>
/// Represents the budget status for a specific expense category.
/// </summary>
public class BudgetStatusResponse
{
	/// <summary>
	/// Gets or sets the name of the expense category.
	/// </summary>
	public ExpenseCategory Category { get; set; }

	/// <summary>
	/// Gets or sets the total amount budgeted for the category.
	/// </summary>
	public required double Budgeted { get; set; }

	/// <summary>
	/// Gets or sets the total amount spent in the category.
	/// </summary>
	public required double Spent { get; set; }

	/// <summary>
	/// Gets the remaining amount in the budget (Budgeted - Spent).
	/// </summary>
	public double Remaining => Budgeted - Spent;

	/// <summary>
	/// Gets the percentage of the budget that has been used.
	/// Returns 0 if Budgeted is 0. Rounded to 2 decimal places.
	/// </summary>
	public double PercentageUsed => Budgeted == 0 ? 0 : Math.Round(Spent / Budgeted * 100, 2);
}
