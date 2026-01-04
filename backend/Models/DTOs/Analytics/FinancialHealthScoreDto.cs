namespace ExpenseTracker.Models.DTOs.Analytics;

/// <summary>
/// Represents the financial health score breakdown for a user (0-100 scale)
/// </summary>
public class FinancialHealthScoreDto
{
    /// <summary>
    /// Total health score (0-100)
    /// </summary>
    public int TotalScore { get; set; }

    /// <summary>
    /// Savings rate score (0-30 points)
    /// </summary>
    public int SavingsScore { get; set; }

    /// <summary>
    /// Budget adherence score (0-25 points)
    /// </summary>
    public int BudgetScore { get; set; }

    /// <summary>
    /// Goal progress score (0-20 points)
    /// </summary>
    public int GoalScore { get; set; }

    /// <summary>
    /// Spending trend score (0-15 points)
    /// </summary>
    public int TrendScore { get; set; }

    /// <summary>
    /// Emergency fund score (0-10 points)
    /// </summary>
    public int EmergencyScore { get; set; }

    /// <summary>
    /// Rating: Excellent, Good, Fair, or Poor
    /// </summary>
    public string Rating { get; set; } = string.Empty;

    /// <summary>
    /// Trend direction: improving, stable, or declining
    /// </summary>
    public string Trend { get; set; } = "stable";

    /// <summary>
    /// Personalized recommendations to improve score
    /// </summary>
    public List<string> Recommendations { get; set; } = [];
}
