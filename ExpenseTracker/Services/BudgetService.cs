using ExpenseTracker.Models;
using ExpenseTracker.Enums;
using ExpenseTracker.Data;
using Microsoft.EntityFrameworkCore;
using ExpenseTracker.Models.DTOs.Budget;
using ExpenseTracker.Models.DTOs.Budgets;

namespace ExpenseTracker.Services;

/// <summary>
/// Provides budget management services, including creating, updating, retrieving, and deleting budgets,
/// as well as calculating spent amounts and generating budget overviews for users.
/// </summary>
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
                b.Period.Year == request.Period.Year
            );

            if (existingBudget is not null)
            {
                return ServiceResult<CreateBudgetResponse?>.Fail(null, null, ["Budget already exists for this category and month."]);
            }

            Budget createdBudget = new()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Category = request.Category,
                Limit = request.Limit,
                Period = new DateOnly(request.Period.Year, request.Period.Month, 1),
            };

            await _dbContext.Budgets.AddAsync(createdBudget);
            await _dbContext.SaveChangesAsync();

            return ServiceResult<CreateBudgetResponse?>.Ok(new CreateBudgetResponse
            {
                Id = createdBudget.Id,
                Category = createdBudget.Category,
                Limit = createdBudget.Limit,
                Period = createdBudget.Period,
                CreatedAt = createdBudget.CreatedAt
            });
        }
        catch (Exception ex)
        {
            return ServiceResult<CreateBudgetResponse?>.Fail(null, null, [ex.Message]);
        }
    }

    public async Task<double> GetSpentAmountForCategoryAsync(Guid userId, ExpenseCategory category, DateOnly date)
    {
        return await _dbContext.Expenses
            .Where(e => e.UserId == userId &&
                        e.Category == category &&
                        e.DateOfExpense.Month == date.Month &&
                        e.DateOfExpense.Year == date.Year)
            .SumAsync(e => e.Amount);
    }

    public async Task<BudgetStatusResponse> GetBudgetStatusAsync(Guid userId, BudgetStatusRequest request)
    {
        if (request.Category is null || request.Period is null)
        {
            throw new ArgumentException("Category and Period must be provided.");
        }

        Budget? budget = await _dbContext.Budgets.FirstOrDefaultAsync(b =>
            b.UserId == userId &&
            b.Category == request.Category &&
            b.Period.Month == request.Period.Value.Month &&
            b.Period.Year == request.Period.Value.Year);

        double spent = await GetSpentAmountForCategoryAsync(userId, request.Category.Value, request.Period.Value);

        return new BudgetStatusResponse
        {
            Id = budget!.Id,
            BudgetedAmount = budget?.Limit ?? 0,
            SpentAmount = spent,
        };
    }

    public async Task<BudgetOverviewSummaryResponse> GetBudgetOverviewAsync(Guid userId, DateOnly period)
    {
        ExpenseCategory[] categories = Enum.GetValues<ExpenseCategory>();
        List<CategoryBudgetOverviewDto> overview = [];

        foreach (ExpenseCategory category in categories)
        {
            BudgetStatusRequest request = new()
            {
                Category = category,
                Period = period,
            };

            BudgetStatusResponse status = await GetBudgetStatusAsync(userId, request);

            double percentage = status.BudgetedAmount == 0
                ? 0
                : Math.Round(status.SpentAmount / status.BudgetedAmount * 100, 2);

            if (percentage == 0) continue;

            overview.Add(new CategoryBudgetOverviewDto
            {
                Category = category,
                PercentageUsed = percentage,
            });
        }

        return new BudgetOverviewSummaryResponse
        {
            Period = new DateOnly(period.Year, period.Month, 1),
            Categories = overview,
        };
    }

    public async Task<ServiceResult<object?>> UpdateBudgetAsync(Guid userId, UpdateBudgetRequest request)
    {
        try
        {
            Budget? budget = await _dbContext.Budgets
                .FirstOrDefaultAsync(b => b.UserId == userId && b.Id == request.BudgetId);

            if (budget is null)
            {
                return ServiceResult<object?>.Fail(null, null, ["Budget not found."]);
            }

            // If no new data was provided, skip update
            if (request.Category is null && request.Period is null && request.NewLimit is null)
            {
                return ServiceResult<object?>.Fail(null, null, ["No update values provided."]);
            }

            // Check for duplicate if period/category is changing
            if ((request.Category is not null || request.Period is not null) &&
                (request.Category != budget.Category || request.Period != budget.Period))
            {
                ExpenseCategory newCategory = request.Category ?? budget.Category;
                DateOnly newPeriod = request.Period ?? budget.Period;

                bool exists = await _dbContext.Budgets.AnyAsync(b =>
                    b.UserId == userId &&
                    b.Id != budget.Id &&
                    b.Category == newCategory &&
                    b.Period.Month == newPeriod.Month &&
                    b.Period.Year == newPeriod.Year
                );

                if (exists)
                {
                    return ServiceResult<object?>.Fail(null, null, ["Another budget already exists for this category and period."]);
                }

                budget.Category = newCategory;
                budget.Period = new DateOnly(newPeriod.Year, newPeriod.Month, 1);
            }

            if (request.NewLimit is not null)
            {
                budget.Limit = request.NewLimit.Value;
            }

            await _dbContext.SaveChangesAsync();
            return ServiceResult<object?>.Ok(null, "Budget updated successfully.");
        }
        catch (Exception ex)
        {
            return ServiceResult<object?>.Fail(null, null, [ex.Message]);
        }
    }

    public async Task<ServiceResult<object?>> DeleteBudgetAsync(Guid userId, Guid budgetId)
    {
        try
        {
            Budget? budget = await _dbContext.Budgets.FirstOrDefaultAsync(b =>
                b.UserId == userId &&
                b.Id == budgetId);

            if (budget is null)
            {
                return ServiceResult<object?>.Fail(null, null, ["Budget not found."]);
            }

            _dbContext.Budgets.Remove(budget);
            await _dbContext.SaveChangesAsync();

            return ServiceResult<object?>.Ok(null, "Budget deleted successfully.");
        }
        catch (Exception ex)
        {
            return ServiceResult<object?>.Fail(null, null, [ex.Message]);
        }
    }
}
