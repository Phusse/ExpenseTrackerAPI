using ExpenseTracker.Enums;

namespace ExpenseTracker.Models.DTOs.Dashboards;

/// <summary>
/// Represents the total amount spent in a specific expense category.
/// Used for generating category-based visual summaries such as pie charts.
/// </summary>
public class CategorySpendingDto
{
	/// <summary>
	/// Gets or sets the name of the expense category.
	/// </summary>
	public required ExpenseCategory Category { get; set; }

	/// <summary>
	/// Gets or sets the total amount spent in the specified category.
	/// </summary>
	public required double TotalSpent { get; set; }
}
