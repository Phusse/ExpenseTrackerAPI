using ExpenseTracker.Enums;

namespace ExpenseTracker.Models.DTOs.Analytics;

/// <summary>
/// Represents a category spending trend
/// </summary>
public class CategoryTrendDto
{
    public ExpenseCategory Category { get; set; }
    public double CurrentMonthTotal { get; set; }
    public double LastMonthTotal { get; set; }
    public double ChangePercentage { get; set; }
    public string Trend { get; set; } = "stable"; // increasing, decreasing, stable
    public double AverageTransactionSize { get; set; }
    public int TransactionCount { get; set; }
}
