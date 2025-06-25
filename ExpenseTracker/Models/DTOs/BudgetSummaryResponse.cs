namespace ExpenseTracker.Models.DTOs;

public class BudgetSummaryResponse
{
    public double BudgetedAmount { get; set; }
    public double SpentAmount { get; set; }
    public double PercentageUsed { get; set; }
    public string Message { get; set; } = string.Empty;
}
