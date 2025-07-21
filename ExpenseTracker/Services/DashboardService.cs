using ExpenseTracker.Data;
using ExpenseTracker.Enums;
using ExpenseTracker.Models;
using ExpenseTracker.Models.DTOs.Dashboard;
using ExpenseTracker.Services;
using Microsoft.EntityFrameworkCore;

internal class DashboardService(ExpenseTrackerDbContext dbContext) : IDashboardService
{
    private readonly ExpenseTrackerDbContext _dbContext = dbContext;

    public async Task<DashboardSummaryResponse> GetDashboardSummaryAsync(Guid userId)
    {
        DateTime now = DateTime.UtcNow;
        int currentMonth = now.Month;
        int currentYear = now.Year;

        // Total expenses
        double totalExpenses = await _dbContext.Expenses
            .Where(e => e.UserId == userId &&
                        e.DateOfExpense.HasValue &&
                        e.DateOfExpense.Value.Month == currentMonth &&
                        e.DateOfExpense.Value.Year == currentYear)
            .SumAsync(e => e.Amount);

        // Total savings
        double totalSavings = await _dbContext.Expenses
            .Where(e => e.UserId == userId &&
                        e.Category == ExpenseCategory.Savings &&
                        e.DateOfExpense.HasValue &&
                        e.DateOfExpense.Value.Month == currentMonth &&
                        e.DateOfExpense.Value.Year == currentYear)
            .SumAsync(e => e.Amount);

        // Budgets
        var userBudgets = await _dbContext.Budgets
            .Where(b => b.UserId == userId &&
                        b.Month == currentMonth &&
                        b.Year == currentYear)
            .ToListAsync();

        var expensesByCategory = await _dbContext.Expenses
            .Where(e => e.UserId == userId &&
                        e.DateOfExpense.HasValue &&
                        e.DateOfExpense.Value.Month == currentMonth &&
                        e.DateOfExpense.Value.Year == currentYear)
            .GroupBy(e => e.Category)
            .Select(g => new
            {
                Category = g.Key,
                TotalSpent = g.Sum(e => e.Amount)
            })
            .ToListAsync();

        var spentLookup = expensesByCategory.ToDictionary(e => e.Category, e => e.TotalSpent);

        List<BudgetStatusResponse> budgetStatuses = userBudgets
            .Select(budget => new BudgetStatusResponse
            {
                Category = budget.Category,
                Budgeted = budget.LimitAmount,
                Spent = spentLookup.TryGetValue(budget.Category, out var spent) ? spent : 0
            })
            .ToList();

        // Category breakdown
        List<CategorySpendingResponse> categoryBreakdown = expensesByCategory
            .Select(c => new CategorySpendingResponse
            {
                Category = c.Category.ToString(),
                TotalSpent = c.TotalSpent
            }).ToList();

        // Recent transactions (limit to 5)
        List<RecentTransactionResponse> recentTransactions = await _dbContext.Expenses
            .Where(e => e.UserId == userId && e.DateOfExpense.HasValue)
            .OrderByDescending(e => e.DateOfExpense)
            .Take(5)
            .Select(e => new RecentTransactionResponse
            {
                Id = e.Id,
                Category = e.Category.ToString(),
                Amount = e.Amount,
                DateOfExpense = e.DateOfExpense.Value,
                Description = e.Description
            })
            .ToListAsync();

        // Saving goals
        var savingGoals = await _dbContext.SavingGoals
            .Where(sg => sg.UserId == userId && !sg.IsArchived)
            .OrderByDescending(sg => sg.UpdatedAt)  // or Deadline or CreatedAt
            .Take(3)
            .Select(sg => new SavingGoalProgressResponse
            {
                Title = sg.Title,
                TargetAmount = sg.TargetAmount,
                CurrentAmount = sg.CurrentAmount,
                Deadline = sg.Deadline,
                // ProgressPercent = sg.TargetAmount == 0 ? 0 : Math.Round(sg.CurrentAmount / sg.TargetAmount * 100, 2)
            })
            .ToListAsync();

        // Daily trend for current month
        List<DailySpendingResponse> monthlyExpenseTrends = await _dbContext.Expenses
            .Where(e => e.UserId == userId &&
                        e.DateOfExpense.HasValue &&
                        e.DateOfExpense.Value.Month == currentMonth &&
                        e.DateOfExpense.Value.Year == currentYear)
            .GroupBy(e => e.DateOfExpense.Value.Date)
            .Select(g => new DailySpendingResponse
            {
                Date = g.Key,
                TotalSpent = g.Sum(e => e.Amount)
            })
            .ToListAsync();

        // Final response
        return new DashboardSummaryResponse
        {
            TotalExpenses = totalExpenses,
            TotalSavings = totalSavings,
            Budgets = budgetStatuses,
            CategoryBreakdown = categoryBreakdown,
            RecentTransactions = recentTransactions,
            SavingGoals = savingGoals,
            DailyTrend = monthlyExpenseTrends
        };
    }
}
