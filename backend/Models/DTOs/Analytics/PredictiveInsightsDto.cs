namespace ExpenseTracker.Models.DTOs.Analytics;

/// <summary>
/// Represents predictive insights and smart recommendations
/// </summary>
public class PredictiveInsightsDto
{
    /// <summary>
    /// Budget warnings and alerts
    /// </summary>
    public List<BudgetWarningDto> BudgetWarnings { get; set; } = [];

    /// <summary>
    /// Goal completion predictions
    /// </summary>
    public List<GoalPredictionDto> GoalPredictions { get; set; } = [];

    /// <summary>
    /// Smart recommendations
    /// </summary>
    public List<RecommendationDto> Recommendations { get; set; } = [];

    /// <summary>
    /// Savings opportunities
    /// </summary>
    public List<SavingsOpportunityDto> SavingsOpportunities { get; set; } = [];
}

public class BudgetWarningDto
{
    public string Category { get; set; } = string.Empty;
    public double BudgetLimit { get; set; }
    public double CurrentSpending { get; set;}
    public double ProjectedTotal { get; set; }
    public double ExcessAmount { get; set; }
    public string Severity { get; set; } = "warning"; // info, warning, critical
    public string Message { get; set; } = string.Empty;
}

public class GoalPredictionDto
{
    public string GoalTitle { get; set; } = string.Empty;
    public double TargetAmount { get; set; }
    public double CurrentAmount { get; set; }
    public DateTime? TargetDate { get; set; }
    public DateTime? ProjectedCompletionDate { get; set; }
    public string Status { get; set; } = string.Empty; // on-track, behind, ahead
    public double MonthlyContributionNeeded { get; set; }
    public string Message { get; set; } = string.Empty;
}

public class RecommendationDto
{
    public string Type { get; set; } = string.Empty; // budget, savings, goal
    public string Category { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public double? SuggestedAmount { get; set; }
    public string Priority { get; set; } = "medium"; // low, medium, high
}

public class SavingsOpportunityDto
{
    public string Category { get; set; } = string.Empty;
    public double CurrentSpending { get; set; }
    public double RecommendedReduction { get; set; }
    public double PotentialMonthlySavings { get; set; }
    public string Message { get; set; } = string.Empty;
}
