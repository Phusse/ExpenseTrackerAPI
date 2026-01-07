using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseTracker.Models;

/// <summary>
/// Stores a user's security question and hashed answer
/// </summary>
public class SecurityQuestion
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid UserId { get; set; }

    /// <summary>
    /// The question ID from the predefined list (1-10)
    /// </summary>
    [Required]
    public int QuestionId { get; set; }

    /// <summary>
    /// BCrypt hashed answer (stored lowercase, trimmed)
    /// </summary>
    [Required]
    public required string AnswerHash { get; set; }

    /// <summary>
    /// Order of the question (1, 2, or 3)
    /// </summary>
    public int QuestionOrder { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [ForeignKey("UserId")]
    public virtual User? User { get; set; }
}

/// <summary>
/// Predefined security questions
/// </summary>
public static class SecurityQuestions
{
    public static readonly Dictionary<int, string> Questions = new()
    {
        { 1, "What is your mother's maiden name?" },
        { 2, "What was the name of your first pet?" },
        { 3, "What city were you born in?" },
        { 4, "What was the name of your elementary school?" },
        { 5, "What is the name of your favorite teacher?" },
        { 6, "What was your childhood nickname?" },
        { 7, "What is the name of your favorite childhood friend?" },
        { 8, "What was the make of your first car?" },
        { 9, "What is your favorite movie?" },
        { 10, "What is the name of the street you grew up on?" }
    };

    public static string GetQuestion(int id) => Questions.TryGetValue(id, out var q) ? q : "Unknown question";
}
