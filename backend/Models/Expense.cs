using System.ComponentModel.DataAnnotations;
using ExpenseTracker.Enums;

namespace ExpenseTracker.Models;

/// <summary>
/// Represents a financial expense recorded by a user.
/// </summary>
public class Expense
{
    /// <summary>
    /// Gets or sets the unique identifier for the expense.
    /// </summary>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the user who made the expense.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the category of the expense (e.g., Food, Transport, Utilities).
    /// </summary>
    [Required(ErrorMessage = "Category is required.")]
    public ExpenseCategory Category { get; set; }

    /// <summary>
    /// Gets or sets the amount of money spent for this expense.
    /// </summary>
    [Required(ErrorMessage = "Amount is required.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0.")]
    public double Amount { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the expense was recorded in the system.
    /// This field is automatically filled by the backend.
    /// </summary>
    public DateTime DateRecorded { get; set; }

    /// <summary>
    /// Gets or sets the actual date when the expense occurred.
    /// This is a required field provided by the user.
    /// </summary>
    [Required(ErrorMessage = "Date of expense is required.")]
    public DateTime DateOfExpense { get; set; }

    /// <summary>
    /// Gets or sets the method of payment used (e.g., Cash, Credit Card, Mobile Payment).
    /// </summary>
    public PaymentMethod PaymentMethod { get; set; }

    /// <summary>
    /// Gets or sets an optional description of the expense.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the user associated with the expense.
    /// </summary>
    public virtual User? User { get; set; }

    /// <summary>
    /// Gets or sets the list of saving goal contributions associated with this expense.
    /// </summary>
    public virtual ICollection<SavingGoalContribution> Contributions { get; set; } = [];
}
