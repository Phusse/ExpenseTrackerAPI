using System.ComponentModel.DataAnnotations;
using ExpenseTracker.Enums;

namespace ExpenseTracker.Models.DTOs.SavingGoals;

/// <summary>
/// DTO for adding a contribution to a saving goal.
/// </summary>
public class AddSavingContributionRequest
{
	/// <summary>
	/// The unique identifier of the saving goal.
	/// </summary>
	[Required]
	public Guid SavingGoalId { get; set; }

	/// <summary>
	/// The amount to contribute.
	/// </summary>
	[Required, Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
	public double Amount { get; set; }

	/// <summary>
	/// The date of the contribution (optional).
	/// </summary>
	public DateTime? DateOfExpense { get; set; }

	/// <summary>
	/// Additional details about the contribution (optional).
	/// </summary>
	public string? Description { get; set; }

	/// <summary>
	/// The payment method used.
	/// </summary>
	[Required]
	public required PaymentMethod PaymentMethod { get; set; }
}
