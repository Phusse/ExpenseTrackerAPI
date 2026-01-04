namespace ExpenseTracker.Models.DTOs.Analytics;

/// <summary>
/// Represents spending patterns analysis
/// </summary>
public class SpendingPatternsDto
{
    /// <summary>
    /// Average spending by day of week
    /// </summary>
    public Dictionary<string, double> SpendingByDayOfWeek { get; set; } = [];

    /// <summary>
    /// Category trends with month-over-month changes
    /// </summary>
    public List<CategoryTrendDto> CategoryTrends { get; set; } = [];

    /// <summary>
    /// Detected recurring expenses
    /// </summary>
    public List<RecurringExpenseDto> RecurringExpenses { get; set; } = [];

    /// <summary>
    /// Unusual or anomalous spending detected
    /// </summary>
    public List<AnomalyDto> Anomalies { get; set; } = [];
}
