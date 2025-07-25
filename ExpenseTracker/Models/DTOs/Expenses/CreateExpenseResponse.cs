using ExpenseTracker.Enums;

namespace ExpenseTracker.Models.DTOs.Expenses;

/// <summary>
/// Represents an expense as returned from the service.
/// </summary>
public class CreateExpenseResponse
{
	/// <summary>
	/// The unique identifier of the expense.
	/// </summary>
	public Guid Id { get; set; }

	/// <summary>
	/// The category of the expense (e.g., Food, Transport, Utilities).
	/// </summary>
	public ExpenseCategory Category { get; set; }

	/// <summary>
	/// The amount of money spent.
	/// </summary>
	public double Amount { get; set; }

	/// <summary>
	/// The timestamp when the expense was recorded in the system.
	/// </summary>
	public DateTime DateRecorded { get; set; }

	/// <summary>
	/// The actual date the expense occurred.
	/// </summary>
	public DateTime DateOfExpense { get; set; }

	/// <summary>
	/// The payment method used for the expense.
	/// </summary>
	public PaymentMethod PaymentMethod { get; set; }

	/// <summary>
	/// A description or note about the expense, if provided.
	/// </summary>
	public string? Description { get; set; }
}
