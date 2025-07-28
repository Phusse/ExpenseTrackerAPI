using ExpenseTracker.Data;
using ExpenseTracker.Enums;
using ExpenseTracker.Models;
using ExpenseTracker.Models.DTOs.SavingGoals;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Services;

/// <summary>
/// Provides services for managing saving goals using the ExpenseTrackerDbContext.
/// </summary>
internal class SavingGoalService(ExpenseTrackerDbContext dbContext) : ISavingGoalService
{
    private readonly ExpenseTrackerDbContext _dbContext = dbContext;

    public async Task<ServiceResult<CreateSavingGoalResponse?>> CreateGoalAsync(CreateSavingGoalRequest request, Guid userId)
    {
        try
        {
            bool exists = await _dbContext.SavingGoals
                .AnyAsync(g => g.UserId == userId && g.Title.ToLower() == request.Title.ToLower() && !g.IsArchived);

            if (exists)
            {
                return ServiceResult<CreateSavingGoalResponse?>.Fail(null, "A saving goal with the same title already exists.");
            }

            SavingGoal goal = new()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Title = request.Title.Trim(),
                Description = request.Description?.Trim(),
                TargetAmount = request.TargetAmount,
                Deadline = request.Deadline,
                Status = SavingGoalStatus.Active,
            };

            await _dbContext.SavingGoals.AddAsync(goal);
            await _dbContext.SaveChangesAsync();

            var payload = CreateSavingGoalResponse.MapSavingGoal(goal);

            return ServiceResult<CreateSavingGoalResponse?>.Ok(payload, "Saving goal created successfully.");
        }
        catch (Exception ex)
        {
            return ServiceResult<CreateSavingGoalResponse?>.Fail(null, null, [ex.Message]);
        }
    }

    public async Task<CreateSavingGoalResponse?> GetGoalByIdAsync(Guid id, Guid userId)
    {
        SavingGoal? goal = await _dbContext.SavingGoals
            .Include(g => g.Contributions)
            .FirstOrDefaultAsync(g => g.Id == id && g.UserId == userId);

        if (goal == null) return null;

        return CreateSavingGoalResponse.MapSavingGoal(goal);
    }

    public async Task<IEnumerable<CreateSavingGoalResponse>> GetAllGoalsAsync(Guid userId, bool includeArchived = false)
    {
        List<CreateSavingGoalResponse> result = await _dbContext.SavingGoals
            .Where(g => g.UserId == userId && (includeArchived || !g.IsArchived))
            .OrderByDescending(g => g.CreatedAt)
            .Select(g => CreateSavingGoalResponse.MapSavingGoal(g))
            .ToListAsync();

        return result;
    }

    public async Task<ServiceResult<object?>> UpdateGoalAsync(Guid id, UpdateSavingGoalRequest request, Guid userId)
    {
        SavingGoal? goal = await _dbContext.SavingGoals.FirstOrDefaultAsync(g => g.Id == id && g.UserId == userId);

        if (goal is null)
        {
            return ServiceResult<object?>.Fail(null, "Saving goal not found.");
        }

        goal.Title = request.Title ?? goal.Title;
        goal.Description = request.Description ?? goal.Description;
        goal.TargetAmount = request.TargetAmount ?? goal.TargetAmount;
        goal.Deadline = request.Deadline ?? goal.Deadline;
        goal.Status = request.Status ?? goal.Status;
        goal.IsArchived = request.IsArchived ?? goal.IsArchived;
        goal.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();
        return ServiceResult<object?>.Ok(null, "Saving goal updated successfully.");
    }

    public async Task<ServiceResult<object?>> DeleteGoalAsync(Guid id, Guid userId)
    {
        SavingGoal? goal = await _dbContext.SavingGoals.FirstOrDefaultAsync(g => g.Id == id && g.UserId == userId);

        if (goal is null)
        {
            return ServiceResult<object?>.Fail(null, "Saving goal not found.");
        }

        _dbContext.SavingGoals.Remove(goal);
        await _dbContext.SaveChangesAsync();

        return ServiceResult<object?>.Ok(null, "Saving goal deleted successfully.");
    }

    public async Task<ServiceResult<object?>> ArchiveGoalAsync(Guid id, Guid userId, bool archiveGoal = true)
    {
        SavingGoal? goal = await _dbContext.SavingGoals.FirstOrDefaultAsync(g => g.Id == id && g.UserId == userId);

        if (goal is null)
        {
            return ServiceResult<object?>.Fail(null, "Saving goal not found.");
        }

        if (goal.IsArchived == archiveGoal)
        {
            string state = archiveGoal ? "already archived" : "already unarchived";
            return ServiceResult<object?>.Ok(null, null, [$"Saving goal is {state}."]);
        }

        goal.IsArchived = archiveGoal;
        goal.ArchivedAt = DateTime.UtcNow;
        goal.UpdatedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();

        string action = archiveGoal ? "archived." : "unarchived.";
        return ServiceResult<object?>.Ok(null, $"Saving goal {action}.");
    }

    public async Task<ServiceResult<object?>> AddSavingContributionAsync(AddSavingContributionRequest request, Guid userId)
    {
        SavingGoal? goal = await _dbContext.SavingGoals.FirstOrDefaultAsync(g => g.Id == request.SavingGoalId && g.UserId == userId);

        if (goal is null)
        {
            return ServiceResult<object?>.Fail(null, "Saving goal not found.");
        }

        Expense expense = new()
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Amount = request.Amount,
            Category = ExpenseCategory.Savings,
            DateRecorded = DateTime.UtcNow,
            DateOfExpense = request.DateOfExpense ?? DateTime.UtcNow,
            PaymentMethod = request.PaymentMethod,
            Description = request.Description,
        };

        SavingGoalContribution contribution = new()
        {
            Id = Guid.NewGuid(),
            SavingGoalId = goal.Id,
            ExpenseId = expense.Id,
            Amount = request.Amount,
            ContributedAt = DateTime.UtcNow,
        };

        goal.CurrentAmount += request.Amount;
        goal.UpdatedAt = DateTime.UtcNow;

        await _dbContext.Expenses.AddAsync(expense);
        await _dbContext.SavingGoalContributions.AddAsync(contribution);
        await _dbContext.SaveChangesAsync();

        return ServiceResult<object?>.Ok(null, "Saving contribution added successfully.");
    }
}
