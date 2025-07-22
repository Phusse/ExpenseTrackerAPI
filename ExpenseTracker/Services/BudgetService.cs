using ExpenseTracker.Models;
using ExpenseTracker.Enums;
using ExpenseTracker.Data;
using Microsoft.EntityFrameworkCore;
using ExpenseTracker.Models.DTOs;
using ExpenseTracker.Services;

internal class BudgetService(ExpenseTrackerDbContext dbContext) : IBudgetService
{
    private readonly ExpenseTrackerDbContext _dbContext = dbContext;

    public async Task<(bool IsSuccess, Budget? Data, string? ErrorMessage)> CreateBudgetAsync(CreateBudgetRequest request, Guid userId)
    {
        try
        {
            Budget? existing = await _dbContext.Budgets.FirstOrDefaultAsync(b =>
                b.UserId == userId &&
                b.Category == request.Category &&
                b.Period.Month == request.Month &&
                b.Period.Year == request.Year);

            if (existing != null)
                return (false, null, "Budget already exists for this category and month.");

            var budget = new Budget
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Category = request.Category,
                Limit = request.Limit,
                Period = new DateOnly(request.Year, request.Month, 1) // 1st day of the month
            };

            await _dbContext.Budgets.AddAsync(budget);
            await _dbContext.SaveChangesAsync();

            return (true, budget, null);
        }
        catch (Exception ex)
        {
            return (false, null, ex.Message);
        }
    }

    public async Task<double> GetSpentAmountForCategoryAsync(Guid userId, ExpenseCategory category, int month, int year)
    {
        return await _dbContext.Expenses
            .Where(e => e.UserId == userId &&
                        e.Category == category &&
                        e.DateOfExpense.Month == month &&
                        e.DateOfExpense.Year == year)
            .SumAsync(e => e.Amount);
    }

    public async Task<(double budgetedAmount, double spentAmount)> GetBudgetStatusAsync(Guid userId, ExpenseCategory category, int month, int year)
    {
        var budget = await _dbContext.Budgets.FirstOrDefaultAsync(b =>
            b.UserId == userId &&
            b.Category == category &&
            b.Period.Month == month &&
            b.Period.Year == year);

        var spent = await GetSpentAmountForCategoryAsync(userId, category, month, year);
        return (budget?.Limit ?? 0, spent);
    }

    public async Task<BudgetSummaryResponse> GetBudgetSummaryAsync(Guid userId, ExpenseCategory category, int month, int year)
    {
        var (budgeted, spent) = await GetBudgetStatusAsync(userId, category, month, year);

        double percentage = budgeted > 0 ? (spent / budgeted) * 100 : 0;
        string message;

        if (budgeted == 0)
        {
            message = "No budget set for this category.";
        }
        else if (percentage >= 100)
        {
            message = "You have exceeded your budget!";
        }
        else if (percentage >= 70)
        {
            message = "You have used more than 70% of your budget.";
        }
        else
        {
            message = "You are within your budget.";
        }

        return new BudgetSummaryResponse
        {
            BudgetedAmount = budgeted,
            SpentAmount = spent,
            PercentageUsed = Math.Round(percentage, 2),
            Message = message
        };
    }
}
