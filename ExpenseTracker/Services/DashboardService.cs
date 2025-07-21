using ExpenseTracker.Data;
using ExpenseTracker.Enums;
using ExpenseTracker.Models.DTOs.Dashboard;
using ExpenseTracker.Services;
using Microsoft.EntityFrameworkCore;

public class DashboardService : IDashboardService
{
    private readonly ExpenseTrackerDbContext _dbContext;

    public DashboardService(ExpenseTrackerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<DashboardSummaryResponse> GetDashboardSummaryAsync(Guid userId)
    {
        var now = DateTime.UtcNow;
        var currentMonth = now.Month;
        var currentYear = now.Year;

        var totalExpenses = await _dbContext.Expenses
     .Where(e => e.UserId == userId &&
                 e.DateOfExpense.HasValue &&
                 e.DateOfExpense.Value.Month == currentMonth &&
                 e.DateOfExpense.Value.Year == currentYear)
     .SumAsync(e => e.Amount);

        var totalSavings = await _dbContext.Expenses
            .Where(e => e.UserId == userId &&
                        e.Category == ExpenseCategory.Savings &&
                        e.DateOfExpense.HasValue &&
                        e.DateOfExpense.Value.Month == currentMonth &&
                        e.DateOfExpense.Value.Year == currentYear)
            .SumAsync(e => e.Amount);


        // 3. Budget health per category
        var categories = Enum.GetValues(typeof(ExpenseCategory)).Cast<ExpenseCategory>();

        var budgetStatuses = new List<BudgetStatusResponse>();

        foreach (var category in categories)
        {
            var budget = await _dbContext.Budgets.FirstOrDefaultAsync(b =>
                b.UserId == userId &&
                b.Category == category &&
                b.Month == currentMonth &&
                b.Year == currentYear);

            var spent = await _dbContext.Expenses
    .Where(e => e.UserId == userId &&
                e.Category == category &&
                e.DateOfExpense.HasValue &&
                e.DateOfExpense.Value.Month == currentMonth &&
                e.DateOfExpense.Value.Year == currentYear)
    .SumAsync(e => e.Amount);


            if (budget != null)
            {
                budgetStatuses.Add(new BudgetStatusResponse
                {
                    // Category = category.ToString(),
                    Budgeted = budget.LimitAmount,
                    Spent = spent
                });
            }
        }

        return new DashboardSummaryResponse
        {
            TotalExpenses = totalExpenses,
            TotalSavings = totalSavings,
            Budgets = budgetStatuses
        };
    }
}
