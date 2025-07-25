using System.ComponentModel.DataAnnotations;
using ExpenseTracker.Enums;

namespace ExpenseTracker.Models;

public class Expense
{
    [Key] public Guid Id { get; set; }
    [Required]
    public ExpenseCategory Category { get; set; }
    [Required]
    public double Amount { get; set; }
    public DateTime DateRecorded { get; set; } // Auto-filled by backend
    [Required(ErrorMessage = "The date of expense must be filled")]
    public DateTime? DateOfExpense { get; set; } // Optional field from user
    public string PaymentMethod { get; set; }

    public string? Description { get; set; }
    [Required]
    public Guid UserId { get; set; }

    // Navigation property
    public virtual User? User { get; set; }
    public virtual ICollection<SavingGoalContribution> Contributions { get; set; } = [];
}
