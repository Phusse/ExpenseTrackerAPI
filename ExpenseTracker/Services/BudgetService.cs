using ExpenseTracker.Models;
using ExpenseTracker.Enums;
using ExpenseTracker.Data;
using Microsoft.EntityFrameworkCore;
using ExpenseTracker.Models.DTOs;

public class BudgetService : IBudgetService
{
    private readonly ExpenseTrackerDbContext _dbContext;

    public BudgetService(ExpenseTrackerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    // Create a new monthly budget for a category
    public async Task<(bool IsSuccess, Budget? Data, string? ErrorMessage)> CreateBudgetAsync(CreateBudgetRequest request, Guid userId)
    {
        try
        {
            var existing = await _dbContext.Budgets.FirstOrDefaultAsync(b =>
                b.UserId == userId &&
                b.Category == request.Category &&
                b.Month == request.Month &&
                b.Year == request.Year);

            if (existing != null)
                return (false, null, "Budget already exists for this category and month.");

            var budget = new Budget
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Category = request.Category,
                LimitAmount = request.LimitAmount,
                Month = request.Month,
                Year = request.Year
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

    // Get how much the user has spent in a given category for the month
    public async Task<double> GetSpentAmountForCategoryAsync(Guid userId, ExpenseCategory category, int month, int year)
    {
        return await _dbContext.Expenses
            .Where(e => e.UserId == userId &&
                        e.Category == category &&
                        e.DateOfExpense != null &&
                        e.DateOfExpense.Value.Month == month &&
                        e.DateOfExpense.Value.Year == year)
            .SumAsync(e => e.Amount);
    }

    // Get the budget amount and amount spent for a given category/month/year
    public async Task<(double budgetedAmount, double spentAmount)> GetBudgetStatusAsync(Guid userId, ExpenseCategory category, int month, int year)
    {
        var budget = await _dbContext.Budgets.FirstOrDefaultAsync(b =>
            b.UserId == userId &&
            b.Category == category &&
            b.Month == month &&
            b.Year == year);

        var spent = await GetSpentAmountForCategoryAsync(userId, category, month, year);
        return (budget?.LimitAmount ?? 0, spent);
    }

    // 🔥 Get status with percentage spent and a warning message if needed
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
