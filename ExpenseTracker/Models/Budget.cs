using System.ComponentModel.DataAnnotations;
using ExpenseTracker.Enums;

namespace ExpenseTracker.Models;

public class Budget
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid UserId { get; set; }

    [Required]
    public ExpenseCategory Category { get; set; }

    [Required]
    public double LimitAmount { get; set; }

    [Required]
    public int Year { get; set; }

    [Required]
    public int Month { get; set; } // 1-12

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User? User { get; set; }
}
