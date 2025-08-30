using ExpenseTracker.Data;
using ExpenseTracker.Enums;
using ExpenseTracker.Models;
using ExpenseTracker.Models.DTOs.Budgets;
using ExpenseTracker.Models.DTOs.Dashboards;
using ExpenseTracker.Services;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Provides dashboard-related services using the ExpenseTrackerDbContext.
/// </summary>
internal class DashboardService(ExpenseTrackerDbContext dbContext) : IDashboardService
{
    private readonly ExpenseTrackerDbContext _dbContext = dbContext;

    public async Task<DashboardSummaryResponse> GetDashboardSummaryAsync(Guid userId)
    {
        DateTime currentTime = DateTime.UtcNow;
        int currentMonth = currentTime.Month;
        int currentYear = currentTime.Year;

        // Total expenses
        double totalExpenses = await _dbContext.Expenses
            .Where(e =>
                e.UserId == userId &&
                e.DateOfExpense.Month == currentMonth &&
                e.DateOfExpense.Year == currentYear
            ).SumAsync(e => e.Amount);

        // Total savings
        double totalSavings = await _dbContext.Expenses
            .Where(e =>
                e.UserId == userId &&
                e.Category == ExpenseCategory.Savings &&
                e.DateOfExpense.Month == currentMonth &&
                e.DateOfExpense.Year == currentYear
            ).SumAsync(e => e.Amount);

        // Budgets
        List<Budget> userBudgets = await _dbContext.Budgets
            .Where(b =>
                b.UserId == userId &&
                b.Period.Month == currentMonth &&
                b.Period.Year == currentYear
            ).ToListAsync();

        // Expenses by category
        List<CategorySpendingDto> expensesByCategory = await _dbContext.Expenses
            .Where(e =>
                e.UserId == userId &&
                e.DateOfExpense.Month == currentMonth &&
                e.DateOfExpense.Year == currentYear
            ).GroupBy(e => e.Category)
            .Select(g => new CategorySpendingDto
            {
                Category = g.Key,
                TotalSpent = g.Sum(e => e.Amount),
            }).ToListAsync();

        Dictionary<ExpenseCategory, double> spentLookup = expensesByCategory.ToDictionary(e => e.Category, e => e.TotalSpent);

        // Budget statuses (limit to 5)
        List<BudgetSummaryResponse> budgetStatuses = [.. userBudgets
            .Select(budget => new BudgetSummaryResponse
            {
                Id = budget.Id,
                Category = budget.Category,
                BudgetedAmount = budget.Limit,
                SpentAmount = spentLookup.TryGetValue(budget.Category, out var spent) ? spent : 0,
                Period = new DateOnly(budget.Period.Year, budget.Period.Month, 1),
            }).OrderByDescending(b => b.PercentageUsed).Take(5)];

        // Recent transactions (limit to 5)
        List<RecentTransactionDto> recentTransactions = await _dbContext.Expenses
            .Where(e => e.UserId == userId)
            .OrderByDescending(e => e.DateOfExpense)
            .Take(5)
            .Select(e => new RecentTransactionDto
            {
                Id = e.Id,
                Category = e.Category.ToString(),
                Amount = e.Amount,
                DateOfExpense = e.DateOfExpense,
                Description = e.Description
            })
            .ToListAsync();

        // Saving goals
        var savingGoals = await _dbContext.SavingGoals
            .Where(sg => sg.UserId == userId && !sg.IsArchived)
            .OrderByDescending(sg => sg.UpdatedAt)
            .Take(3)
            .Select(sg => new SavingGoalProgressDto
            {
                Title = sg.Title,
                TargetAmount = sg.TargetAmount,
                CurrentAmount = sg.CurrentAmount,
                Deadline = sg.Deadline,
            })
            .ToListAsync();

        // Daily trend for current month
        List<DailySpendingDto> monthlyExpenseTrends = await _dbContext.Expenses
            .Where(e =>
                e.UserId == userId &&
                e.DateOfExpense.Month == currentMonth &&
                e.DateOfExpense.Year == currentYear
            )
            .GroupBy(e => e.DateOfExpense.Date)
            .Select(g => new DailySpendingDto
            {
                Date = new(g.Key.Year, g.Key.Month, g.Key.Day),
                TotalSpent = g.Sum(e => e.Amount),
            })
            .ToListAsync();

        // Final response
        return new DashboardSummaryResponse
        {
            TotalExpenses = totalExpenses,
            TotalSavings = totalSavings,
            Budgets = budgetStatuses,
            CategoryBreakdown = expensesByCategory,
            RecentTransactions = recentTransactions,
            SavingGoals = savingGoals,
            DailyTrend = monthlyExpenseTrends
        };
    }
}
