public class DashboardSummaryDto
{
    public double TotalExpenses { get; set; }
    public double TotalSavings { get; set; }
    public List<BudgetStatusDto> Budgets { get; set; } = new();
}

public class BudgetStatusDto
{
    public string Category { get; set; }
    public double Budgeted { get; set; }
    public double Spent { get; set; }
    public double Remaining => Budgeted - Spent;
    public double PercentageUsed => Budgeted == 0 ? 0 : Math.Round((Spent / Budgeted) * 100, 2);
}

