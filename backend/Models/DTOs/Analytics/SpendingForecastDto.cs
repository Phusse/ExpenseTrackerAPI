namespace ExpenseTracker.Models.DTOs.Analytics;

/// <summary>
/// Represents spending forecasts and predictions
/// </summary>
public class SpendingForecastDto
{
    /// <summary>
    /// Projected month-end total
    /// </summary>
    public double ProjectedMonthEnd { get; set; }

    /// <summary>
    /// Current spending to date
    /// </summary>
    public double CurrentSpending { get; set; }

    /// <summary>
    /// Days elapsed in current month
    /// </summary>
    public int DaysElapsed { get; set; }

    /// <summary>
    /// Days remaining in current month
    /// </summary>
    public int DaysRemaining { get; set; }

    /// <summary>
    /// Daily average spending
    /// </summary>
    public double DailyAverage { get; set; }

    /// <summary>
    /// Projected additional spending for rest of month
    /// </summary>
    public double ProjectedAdditionalSpending { get; set; }

    /// <summary>
    /// Category-specific forecasts
    /// </summary>
    public List<CategoryForecastDto> CategoryForecasts { get; set; } = [];
}

public class CategoryForecastDto
{
    public string Category { get; set; } = string.Empty;
    public double Current { get; set; }
    public double Projected { get; set; }
    public double BudgetLimit { get; set; }
    public bool WillExceedBudget { get; set; }
    public double ExcessAmount { get; set; }
}
