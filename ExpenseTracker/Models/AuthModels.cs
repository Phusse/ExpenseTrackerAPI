using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Models;

public class LoginRequest
{
    [Required]
    [EmailAddress]
    public required string Email { get; set; }
    
    [Required]
    [MinLength(6)]
    public required string Password { get; set; }
}

public class RegisterRequest
{
    [Required]
    [StringLength(100)]
    public required string Name { get; set; }
    
    [Required]
    [EmailAddress]
    public required string Email { get; set; }
    
    [Required]
    [MinLength(6)]
    public required string Password { get; set; }
}


