using System.ComponentModel.DataAnnotations;
using ExpenseTracker.Enums;

namespace ExpenseTracker.Models;

/// <summary>
/// Represents a user's savings goal in the expense tracker application.
/// </summary>
/// <remarks>
/// This endpoint authenticates a user with their email and password.
/// On success, it returns a JWT that can be used for future requests.
/// </remarks>
public class SavingGoal
{
    /// <summary>
    /// Gets or sets the unique identifier for the saving goal.
    /// </summary>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the user who owns this saving goal.
    /// </summary>
    [Required]
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the title of the saving goal.
    /// </summary>
    [Required, MaxLength(100)]
    public required string Title { get; set; }

    /// <summary>
    /// Gets or sets an optional description providing more details about the saving goal.
    /// </summary>
    [MaxLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the current amount saved towards the goal.
    /// </summary>
    [Required]
    public double CurrentAmount { get; set; }

    /// <summary>
    /// Gets or sets the target amount to be saved for the goal.
    /// </summary>
    [Required]
    public double TargetAmount { get; set; }

    /// <summary>
    /// Gets or sets the optional deadline by which the goal should be achieved.
    /// </summary>
    public DateTime? Deadline { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the status of the saving goal.
    /// See <see cref="SavingGoalStatus"/> provides the available statuses.
    /// </summary>
    public SavingGoalStatus Status { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the saving goal was created. (defaults to current UTC time).
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the date and time when the saving goal was last updated.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the saving goal is archived.
    /// </summary>
    public bool IsArchived { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the saving goal was archived, if applicable.
    /// </summary>
    public DateTime? ArchivedAt { get; set; }

    /// <summary>
    /// Gets or sets the user who owns this saving goal.
    /// </summary>
    public virtual User? User { get; set; }

    /// <summary>
    /// Gets or sets the collection of contributions made towards this saving goal.
    /// </summary>
    public virtual ICollection<SavingGoalContribution> Contributions { get; set; } = [];
}
