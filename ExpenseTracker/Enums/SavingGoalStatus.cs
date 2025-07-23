using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Enums;

/// <summary>
/// Represents the status of a saving goal, indicating whether it is active, completed, cancelled, or paused.
/// </summary>
public enum SavingGoalStatus
{
	/// <summary>
	/// The goal is currently active and accepting contributions.
	/// </summary>
	[Display(Name = "Active")]
	Active,

	/// <summary>
	/// The goal has been completed — target amount reached.
	/// </summary>
	[Display(Name = "Completed")]
	Completed,

	/// <summary>
	/// The goal was cancelled and is no longer tracked.
	/// </summary>
	[Display(Name = "Cancelled")]
	Cancelled,

	/// <summary>
	/// The goal is paused temporarily and contributions are on hold.
	/// </summary>
	[Display(Name = "Paused")]
	Paused,
}
