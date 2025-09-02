using ExpenseTracker.Enums;

namespace ExpenseTracker.Models.DTOs.Budgets;

/// <summary>
/// Represents a lightweight summary of budget usage for a specific expense category
/// within a given time period. Includes the percentage of budget used and an advisory message.
/// </summary>
public class CategoryBudgetOverviewDto
{
	/// <summary>
	/// The expense category this summary represents.
	/// </summary>
	public ExpenseCategory Category { get; set; }

	/// <summary>
	/// The percentage of the budget that has been used for this category.
	/// </summary>
	public double PercentageUsed { get; set; }

	/// <summary>
	/// A contextual message based on the <see cref="PercentageUsed"/>:
	/// <list type="bullet">
	///   <item><description>"You have exceeded your budget!" if percentage > 100</description></item>
	///   <item><description>"You have reached your budget limit." if percentage == 100</description></item>
	///   <item><description>"You have used more than 70% of your budget." if percentage >= 70</description></item>
	///   <item><description>"You are within your budget." if percentage > 0</description></item>
	///   <item><description>"No budget set for this category." otherwise</description></item>
	/// </list>
	/// </summary>
	public string Message => PercentageUsed switch
	{
		> 100 => "You have exceeded your budget!",
		100 => "You have reached your budget limit.",
		>= 70 => "You have used more than 70% of your budget.",
		> 0 => "You are within your budget.",
		_ => "No expenses recorded for this budget yet.",
	};
}
