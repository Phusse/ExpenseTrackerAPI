using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Models.DTOs.SavingGoals;

/// <summary>
/// DTO for creating a new saving goal.
/// </summary>
public class CreateSavingGoalRequest
{
    /// <summary>
    /// The name of the saving goal.
    /// </summary>
    [Required]
    public required string Title { get; set; }

    /// <summary>
    /// Additional details about the goal.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// The amount to be saved.
    /// </summary>
    [Required, Range(0.01, double.MaxValue, ErrorMessage = "Target amount must be greater than zero.")]
    public double TargetAmount { get; set; }

    /// <summary>
    /// The target date to reach the goal.
    /// </summary>
    public DateTime? Deadline { get; set; }
}
