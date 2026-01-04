namespace ExpenseTracker.Models.DTOs.Analytics;

/// <summary>
/// Represents an unusual spending anomaly
/// </summary>
public class AnomalyDto
{
    public DateTime Date { get; set; }
    public string Category { get; set; } = string.Empty;
    public double Amount { get; set; }
    public double AverageAmount { get; set; }
    public double DeviationPercentage { get; set; }
    public string Reason { get; set; } = string.Empty;
}
