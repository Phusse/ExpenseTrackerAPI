using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Models;

public class Expense
{
    [Key] public Guid Id { get; set; }
    [Required]
    public required string Category { get; set; }
    [Required]
    public double Amount { get; set; }
    [Required]
    public DateTime Date { get; set; }
    public string PaymentMethod{get; set; }

    public string? Description { get; set; }
}
