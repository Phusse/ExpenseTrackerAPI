using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Models;

/// <summary>
/// Represents a user of the Expense Tracker application.
/// </summary>
public class User
{
    /// <summary>
    /// Primary key identifier for the user.
    /// </summary>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// The full name of the user.
    /// </summary>
    [Required(ErrorMessage = "Name is required.")]
    [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters.")]
    public required string Name { get; set; }

    /// <summary>
    /// The user's email address. Must be a valid email format.
    /// </summary>
    [Required(ErrorMessage = "Email address is required.")]
    [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Please enter a valid email address in the format: name@example.com.")]
    [StringLength(255, ErrorMessage = "Email address cannot exceed 255 characters.")]
    public required string Email { get; set; }

    /// <summary>
    /// Hashed password for user authentication.
    /// </summary>
    [Required(ErrorMessage = "Password is required.")]
    public required string PasswordHash { get; set; }

    /// <summary>
    /// Date and time the user account was created (UTC).
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Date and time the user last logged in (UTC), if any.
    /// </summary>
    public DateTime? LastLoginAt { get; set; }

    /// <summary>
    /// Date and time the user last logged out (UTC), if any.
    /// </summary>
    public DateTime? LastLogoutAt { get; set; }

    /// <summary>
    /// Indicates whether the user account is currently active.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Collection of expenses associated with the user.
    /// </summary>
    public virtual ICollection<Expense> Expenses { get; set; } = [];
}
