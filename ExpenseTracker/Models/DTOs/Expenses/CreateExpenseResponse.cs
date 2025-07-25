using ExpenseTracker.Enums;

namespace ExpenseTracker.Models.DTOs.Expenses;

/// <summary>
/// Represents an expense as returned from the service.
/// </summary>
public class CreateExpenseResponse
{
	public Guid Id { get; set; }
	public ExpenseCategory Category { get; set; }
	public double Amount { get; set; }
	public DateTime DateRecorded { get; set; }
	public DateTime DateOfExpense { get; set; }
	public PaymentMethod PaymentMethod { get; set; }
	public string? Description { get; set; }
}
