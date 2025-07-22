using ExpenseTracker.Models;
using ExpenseTracker.Enums;
using ExpenseTracker.Data;
using Microsoft.EntityFrameworkCore;
using ExpenseTracker.Services;
using ExpenseTracker.Models.DTOs.Budget;

internal class BudgetService(ExpenseTrackerDbContext dbContext) : IBudgetService
{
    private readonly ExpenseTrackerDbContext _dbContext = dbContext;

    public async Task<ServiceResult<CreateBudgetResponse?>> CreateBudgetAsync(CreateBudgetRequest request, Guid userId)
    {
        try
        {
            Budget? existingBudget = await _dbContext.Budgets.FirstOrDefaultAsync(b =>
                b.UserId == userId &&
                b.Category == request.Category &&
                b.Period.Month == request.Period.Month &&
                b.Period.Year == request.Period.Year);

            if (existingBudget is not null)
                return ServiceResult<CreateBudgetResponse?>.Fail(null, "Budget already exists for this category and month.");

            Budget createdBudget = new()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Category = request.Category,
                Limit = request.Limit,
                Period = new DateOnly(request.Period.Year, request.Period.Month, 1),    // 1st day of the month
            };

            await _dbContext.Budgets.AddAsync(createdBudget);
            await _dbContext.SaveChangesAsync();

            CreateBudgetResponse gg = new()
            {
                Id = createdBudget.Id,
                Category = createdBudget.Category,
                Limit = createdBudget.Limit,
                Period = createdBudget.Period,
                CreatedAt = createdBudget.CreatedAt,
            };

            return ServiceResult<CreateBudgetResponse?>.Ok(gg);
        }
        catch (Exception ex)
        {
            return ServiceResult<CreateBudgetResponse?>.Fail(null, null, [ex.Message]);
        }
    }

    public async Task<double> GetSpentAmountForCategoryAsync(Guid userId, ExpenseCategory category, DateOnly date)
    {
        return await _dbContext.Expenses.Where(e =>
            e.UserId == userId &&
            e.Category == category &&
            e.DateOfExpense.Month == date.Month &&
            e.DateOfExpense.Year == date.Year
            ).SumAsync(e => e.Amount);
    }

    public async Task<BudgetStatusResponse> GetBudgetStatusAsync(Guid userId, ExpenseCategory category, DateOnly date)
    {
        Budget? budget = await _dbContext.Budgets.FirstOrDefaultAsync(b =>
            b.UserId == userId &&
            b.Category == category &&
            b.Period.Month == date.Month &&
            b.Period.Year == date.Year
        );

        double spent = await GetSpentAmountForCategoryAsync(userId, category, date);

        return new BudgetStatusResponse
        {
            BudgetedAmount = budget?.Limit ?? 0,
            SpentAmount = spent,
        };
    }

    public async Task<BudgetSummaryResponse> GetBudgetSummaryAsync(Guid userId, ExpenseCategory category, DateOnly date)
    {
        BudgetStatusResponse status = await GetBudgetStatusAsync(userId, category, date);

        double budgeted = status.BudgetedAmount;
        double spent = status.SpentAmount;
        double percentage = budgeted > 0 ? spent / budgeted * 100 : 0;

        string message = budgeted switch
        {
            0 => "No budget set for this category.",
            _ => percentage switch
            {
                > 100 => "You have exceeded your budget!",
                100 => "You have reached your budget limit.",
                >= 70 => "You have used more than 70% of your budget.",
                _ => "You are within your budget.",
            }
        };

        return new BudgetSummaryResponse
        {
            Category = category,
            Period = new DateOnly(date.Year, date.Month, 1),
            BudgetedAmount = budgeted,
            SpentAmount = spent,
            PercentageUsed = Math.Round(percentage, 2),
            Message = message,
        };
    }
}
