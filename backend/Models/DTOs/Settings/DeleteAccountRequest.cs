using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Models.DTOs.Settings;

public class DeleteAccountRequest
{
    [Required]
    public string Password { get; set; } = string.Empty;

    public int? SecurityQuestionId { get; set; }

    public string? SecurityAnswer { get; set; }
}
