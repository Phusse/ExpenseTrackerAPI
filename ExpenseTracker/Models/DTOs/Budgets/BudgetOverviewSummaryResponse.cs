namespace ExpenseTracker.Models.DTOs.Budgets;

/// <summary>
/// Represents a summary response for a budget overview, including the period and a list of category budget overviews.
/// </summary>
public class BudgetOverviewSummaryResponse
{
	/// <summary>
	/// The period (month and year) for which the budget overview is being requested.
	/// </summary>
	public DateOnly Period { get; set; }

	/// <summary>
	/// A list of category budget overviews for the specified period.
	/// </summary>
	public List<CategoryBudgetOverviewDto> Categories { get; set; } = [];
}
