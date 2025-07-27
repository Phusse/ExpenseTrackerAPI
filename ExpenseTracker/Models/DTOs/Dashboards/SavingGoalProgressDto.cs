namespace ExpenseTracker.Models.DTOs.Dashboards;

/// <summary>
/// Represents a user's savings goal and current progress toward achieving it.
/// </summary>
public class SavingGoalProgressDto
{
	/// <summary>
	/// Gets or sets the title or name of the savings goal.
	/// </summary>
	public required string Title { get; set; }

	/// <summary>
	/// Gets or sets the target amount the user aims to save.
	/// </summary>
	public required double TargetAmount { get; set; }

	/// <summary>
	/// Gets or sets the amount currently saved toward the target.
	/// </summary>
	public required double CurrentAmount { get; set; }

	/// <summary>
	/// Gets the calculated progress as a percentage of the target amount.
	/// Returns 0 if the target amount is zero.
	/// </summary>
	public double ProgressPercent => TargetAmount == 0
		? 0
		: Math.Round(CurrentAmount / TargetAmount * 100, 2);

	/// <summary>
	/// Gets or sets the optional deadline for achieving the savings goal.
	/// </summary>
	public DateTime? Deadline { get; set; }
}
