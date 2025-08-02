namespace ExpenseTracker.Models.DTOs.Budgets;

/// <summary>
/// Represents the current status of a user's budget for a specific category and period,
/// including how much was allocated and how much has been spent.
/// </summary>
public class BudgetStatusResponse
{
	/// <summary>
	/// The unique identifier of the budget status.
	/// </summary>
	public required Guid? Id { get; set; }

	/// <summary>
	/// The total amount allocated to the budget for the specified category and period.
	/// </summary>
	public double BudgetedAmount { get; set; }

	/// <summary>
	/// The total amount spent by the user in the specified category and period.
	/// </summary>
	public double SpentAmount { get; set; }
}
