using ExpenseTracker.Enums;

namespace ExpenseTracker.Models.Responses;

/// <summary>
/// Data Transfer Object for representing a saving goal in API responses.
/// </summary>
public class SavingGoalResponse
{
	public Guid Id { get; set; }
	public Guid UserId { get; set; }
	public string Title { get; set; } = string.Empty;
	public string? Description { get; set; }
	public double CurrentAmount { get; set; }
	public double TargetAmount { get; set; }
	public DateTime? Deadline { get; set; }
	public SavingGoalStatus Status { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime? UpdatedAt { get; set; }
	public bool IsArchived { get; set; }
	public DateTime? ArchivedAt { get; set; }

	/// <summary>
	/// Maps a SavingGoal domain model to a SavingGoalResponse DTO.
	/// </summary>
	/// <param name="goal">The SavingGoal domain model.</param>
	/// <returns>A SavingGoalResponse DTO.</returns>
	public static SavingGoalResponse MapToResponse(SavingGoal goal) => new()
	{
		Id = goal.Id,
		UserId = goal.UserId,
		Title = goal.Title,
		Description = goal.Description,
		CurrentAmount = goal.CurrentAmount,
		TargetAmount = goal.TargetAmount,
		Deadline = goal.Deadline,
		Status = goal.Status,
		CreatedAt = goal.CreatedAt,
		UpdatedAt = goal.UpdatedAt,
		IsArchived = goal.IsArchived,
		ArchivedAt = goal.ArchivedAt
	};
}