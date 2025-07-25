using ExpenseTracker.Enums;

namespace ExpenseTracker.Models.DTOs.SavingGoals;

/// <summary>
/// Represents a safe and simplified response model for a user's saving goal.
/// </summary>
public class CreateSavingGoalResponse
{
	/// <summary>
	/// The unique identifier of the saving goal.
	/// </summary>
	public Guid Id { get; set; }

	/// <summary>
	/// The title of the saving goal.
	/// </summary>
	public required string Title { get; set; }

	/// <summary>
	/// An optional description of the saving goal.
	/// </summary>
	public string? Description { get; set; }

	/// <summary>
	/// The current amount saved towards the goal.
	/// </summary>
	public double CurrentAmount { get; set; }

	/// <summary>
	/// The target amount the user is aiming to save.
	/// </summary>
	public double TargetAmount { get; set; }

	/// <summary>
	/// The optional deadline to reach the saving goal.
	/// </summary>
	public DateTime? Deadline { get; set; }

	/// <summary>
	/// The current status of the saving goal.
	/// </summary>
	public SavingGoalStatus Status { get; set; }

	/// <summary>
	/// The date and time the goal was created.
	/// </summary>
	public DateTime CreatedAt { get; set; }

	/// <summary>
	/// The date and time the goal was last updated.
	/// </summary>
	public DateTime? UpdatedAt { get; set; }

	/// <summary>
	/// Whether the goal is archived.
	/// </summary>
	public bool IsArchived { get; set; }

	/// <summary>
	/// The date and time the goal was archived, if applicable.
	/// </summary>
	public DateTime? ArchivedAt { get; set; }

	/// <summary>
	/// Maps the saving goal to a simplified response model.
	/// </summary>
	/// <returns>A new instance of <see cref="CreateSavingGoalResponse"/> containing the simplified data.</returns>
	public static CreateSavingGoalResponse MapSavingGoal(SavingGoal goal) => new()
	{
		Id = goal.Id,
		Title = goal.Title.Trim(),
		Description = goal.Description?.Trim(),
		CurrentAmount = goal.CurrentAmount,
		TargetAmount = goal.TargetAmount,
		Deadline = goal.Deadline,
		Status = goal.Status,
		CreatedAt = goal.CreatedAt,
	};
}
