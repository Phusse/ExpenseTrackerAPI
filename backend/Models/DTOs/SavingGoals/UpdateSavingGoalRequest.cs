using ExpenseTracker.Enums;

namespace ExpenseTracker.Models.DTOs.SavingGoals;

/// <summary>
/// Represents a request to update a saving goal.
/// All properties are optional and can be set individually.
/// </summary>
public class UpdateSavingGoalRequest
{
	/// <summary>
	/// The title of the saving goal.
	/// </summary>
	public string? Title { get; set; }

	/// <summary>
	/// Additional details about the saving goal.
	/// </summary>
	public string? Description { get; set; }

	/// <summary>
	/// The target amount to save.
	/// </summary>
	public double? TargetAmount { get; set; }

	/// <summary>
	/// The deadline for achieving the saving goal.
	/// </summary>
	public DateTime? Deadline { get; set; }

	/// <summary>
	/// The current status of the saving goal.
	/// </summary>
	public SavingGoalStatus? Status { get; set; }

	/// <summary>
	/// Indicates whether the saving goal is archived.
	/// </summary>
	public bool? IsArchived { get; set; }
}
