using ExpenseTracker.Enums;

namespace ExpenseTracker.Models.DTOs.Expenses;

/// <summary>
/// Represents the data required to create or update an expense.
/// </summary>
public class CreateExpenseRequest
{
	public Guid UserId { get; set; }
	public ExpenseCategory Category { get; set; }
	public double Amount { get; set; }
	public DateTime DateOfExpense { get; set; }
	public PaymentMethod PaymentMethod { get; set; }
	public string? Description { get; set; }
}
