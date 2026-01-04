namespace ExpenseTracker.Models.DTOs.Analytics;

/// <summary>
/// Represents a detected recurring expense
/// </summary>
public class RecurringExpenseDto
{
    public string Description { get; set; } = string.Empty;
    public double Amount { get; set; }
    public string Frequency { get; set; } = string.Empty; // weekly, monthly
    public DateTime LastOccurrence { get; set; }
    public DateTime? NextExpectedDate { get; set; }
}
