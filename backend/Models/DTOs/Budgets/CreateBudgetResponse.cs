using ExpenseTracker.Enums;

namespace ExpenseTracker.Models.DTOs.Budget;

/// <summary>
/// Represents the response returned after successfully creating a budget.
/// </summary>
public class CreateBudgetResponse
{
	/// <summary>
	/// The unique identifier of the newly created budget.
	/// </summary>
	public Guid Id { get; set; }

	/// <summary>
	/// The expense category this budget applies to.
	/// </summary>
	public ExpenseCategory Category { get; set; }

	/// <summary>
	/// The monthly limit set for this category.
	/// </summary>
	public required double Limit { get; set; }

	/// <summary>
	/// The month and year this budget applies to (first day of the month).
	/// </summary>
	public required DateOnly Period { get; set; }

	/// <summary>
	/// When the budget was created (UTC).
	/// </summary>
	public required DateTime CreatedAt { get; set; }
}
