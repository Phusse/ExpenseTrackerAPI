using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Models;

public class User
{
    [Key]
    public Guid Id { get; set; }
    
    [Required]
    [StringLength(100)]
    public required string Name { get; set; }
    
    [Required]
    [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
    ErrorMessage = "Please enter a valid email address.")]
    [StringLength(255)]
    public required string Email { get; set; }
    
    [Required]
    public required string PasswordHash { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? LastLoginAt { get; set; }
    
    public DateTime? LastLogoutAt { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    // Navigation property for expenses
    public virtual ICollection<Expense> Expenses { get; set; } = new List<Expense>();
}