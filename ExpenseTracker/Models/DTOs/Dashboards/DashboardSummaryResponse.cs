using ExpenseTracker.Models.DTOs.Budgets;

namespace ExpenseTracker.Models.DTOs.Dashboards;

/// <summary>
/// Represents a summary of the user's financial dashboard,
/// including expenses, savings, budget statuses, spending breakdowns, and goal progress.
/// </summary>
public class DashboardSummaryResponse
{
    /// <summary>
    /// Gets or sets the total amount of expenses recorded by the user
    /// for the current time period (e.g., current month).
    /// </summary>
    public required double TotalExpenses { get; set; }

    /// <summary>
    /// Gets or sets the total amount categorized as savings by the user
    /// during the current time period.
    /// </summary>
    public required double TotalSavings { get; set; }

    /// <summary>
    /// Gets or sets the list of budget statuses grouped by category.
    /// Each entry includes the budgeted amount, spent amount, and calculated remaining balance.
    /// </summary>
    public List<BudgetSummaryResponse> Budgets { get; set; } = [];

    /// <summary>
    /// Gets or sets the breakdown of total spending grouped by expense category.
    /// Useful for visualizing category proportions in pie charts.
    /// </summary>
    public List<CategorySpendingDto> CategoryBreakdown { get; set; } = [];

    /// <summary>
    /// Gets or sets the trend of daily spending within the selected time period.
    /// Useful for generating line charts of user activity over time.
    /// </summary>
    public List<DailySpendingDto> DailyTrend { get; set; } = [];

    /// <summary>
    /// Gets or sets the list of the user's most recent expense transactions.
    /// </summary>
    public List<RecentTransactionDto> RecentTransactions { get; set; } = [];

    /// <summary>
    /// Gets or sets the list of the user's active or archived savings goals,
    /// including progress toward each goal based on contributions.
    /// </summary>
    public List<SavingGoalProgressDto> SavingGoals { get; set; } = [];
}
