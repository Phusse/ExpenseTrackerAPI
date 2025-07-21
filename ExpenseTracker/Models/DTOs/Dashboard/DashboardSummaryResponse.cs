namespace ExpenseTracker.Models.DTOs.Dashboard;

/// <summary>
/// Represents a summary of the user's financial dashboard, including expenses, savings, and budget statuses.
/// </summary>
public class DashboardSummaryResponse
{
    /// <summary>
    /// Gets or sets the total amount of expenses.
    /// </summary>
    public required double TotalExpenses { get; set; }

    /// <summary>
    /// Gets or sets the total amount of savings.
    /// </summary>
    public required double TotalSavings { get; set; }

    /// <summary>
    /// Gets or sets the list of budget status entries by category.
    /// </summary>
    public List<BudgetStatusResponse> Budgets { get; set; } = [];
}
