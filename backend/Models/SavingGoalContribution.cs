using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Models;

/// <summary>
/// Represents a contribution made towards a saving goal in the expense tracking application.
/// </summary>
public class SavingGoalContribution
{
	/// <summary>
	/// Unique identifier for the contribution.
	/// </summary>
	[Key]
	public Guid Id { get; set; }

	/// <summary>
	/// Identifier of the related saving goal.
	/// </summary>
	[Required]
	public Guid SavingGoalId { get; set; }

	/// <summary>
	/// Identifier of the related expense.
	/// </summary>
	[Required]
	public Guid ExpenseId { get; set; }

	/// <summary>
	/// Amount contributed.
	/// </summary>
	[Required]
	public double Amount { get; set; }

	/// <summary>
	/// Timestamp of the contribution (defaults to current UTC time).
	/// </summary>
	public DateTime ContributedAt { get; set; } = DateTime.UtcNow;

	/// <summary>
	/// Navigation property to the related saving goal.
	/// </summary>
	public virtual SavingGoal SavingGoal { get; set; } = null!;

	/// <summary>
	/// Navigation property to the related expense.
	/// </summary>
	public virtual Expense Expense { get; set; } = null!;
}