using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Models;

public class Expense
{
    [Key] public Guid Id { get; set; }
    [Required]
    public required string Category { get; set; }
    [Required]
    public double Amount { get; set; }
    public DateTime DateRecorded { get; set; } // Auto-filled by backend
    [Required(ErrorMessage = "The date of expense must be filled")]
    public DateTime? DateOfExpense { get; set; } // Optional field from user
    public string PaymentMethod{get; set; }

    public string? Description { get; set; }
}
