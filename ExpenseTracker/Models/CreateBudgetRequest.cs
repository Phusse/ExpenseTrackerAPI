using System.ComponentModel.DataAnnotations;
using ExpenseTracker.Enums;

public class CreateBudgetRequest
{
    public ExpenseCategory Category { get; set; }
    public double LimitAmount { get; set; }

    [Range(1, 12)]
    public int Month { get; set; } // 1 - 12

    [Range(2020, 2100)]
    public int Year { get; set; }
}
