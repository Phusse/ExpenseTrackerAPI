using System.ComponentModel.DataAnnotations;
using ExpenseTracker.Enums;

namespace ExpenseTracker.Models.DTOs.Expenses;

/// <summary>
/// Represents the data required to update an existing expense.
/// </summary>
public class UpdateExpenseRequest
{
	/// <summary>
	/// The unique identifier of the expense.
	/// </summary>
	[Required]
	public Guid Id { get; set; }

	/// <summary>
	/// The category of the expense.
	/// </summary>
	public ExpenseCategory? Category { get; set; }

	/// <summary>
	/// The amount spent.
	/// </summary>
	public double? Amount { get; set; }

	/// <summary>
	/// The date the expense occurred.
	/// </summary>
	public DateTime? DateOfExpense { get; set; }

	/// <summary>
	/// The method of payment used.
	/// </summary>
	public PaymentMethod? PaymentMethod { get; set; }

	/// <summary>
	/// An optional description of the expense.
	/// </summary>
	public string? Description { get; set; }
}
