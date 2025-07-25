using ExpenseTracker.Enums;

namespace ExpenseTracker.Models.DTOs.Expenses;

/// <summary>
/// Represents the data required to create or update an expense.
/// </summary>
public class CreateExpenseRequest
{
    /// <summary>
    /// The category of the expense (e.g., Food, Transport, Utilities).
    /// </summary>
    public ExpenseCategory Category { get; set; }

    /// <summary>
    /// The amount of money spent. Must be a positive number.
    /// </summary>
    public double Amount { get; set; }

    /// <summary>
    /// The actual date the expense occurred.
    /// </summary>
    public DateTime DateOfExpense { get; set; }

    /// <summary>
    /// The method of payment used for the expense (e.g., Cash, Card, Mobile).
    /// </summary>
    public PaymentMethod PaymentMethod { get; set; }

    /// <summary>
    /// An optional description providing more context about the expense.
    /// </summary>
    public string? Description { get; set; }
}
