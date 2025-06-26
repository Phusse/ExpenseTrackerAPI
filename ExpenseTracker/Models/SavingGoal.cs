using ExpenseTracker.Models;

public class SavingGoal
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public double TargetAmount { get; set; }
    public int Month { get; set; }   // 1 - 12
    public int Year { get; set; }    // e.g., 2025
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User? User { get; set; }
}
