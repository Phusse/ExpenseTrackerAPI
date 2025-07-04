using ExpenseTracker.Data;
using ExpenseTracker.Enums;
using ExpenseTracker.Models;
using ExpenseTracker.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Services;

public class SavingGoalService(ExpenseTrackerDbContext dbContext) : ISavingGoalService
{
    private readonly ExpenseTrackerDbContext _dbContext = dbContext;

    public async Task<(bool IsSuccess, SavingGoal? Data, string? ErrorMessage)> CreateGoalAsync(CreateSavingGoalRequest request, Guid userId)
    {
        try
        {
            bool exists = await _dbContext.SavingGoals
                .AnyAsync(g => g.UserId == userId && g.Title == request.Title && !g.IsArchived);

            if (exists) return (false, null, "You already have a similar saving goal.");

            SavingGoal goal = new()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Title = request.Title,
                Description = request.Description,
                TargetAmount = request.TargetAmount,
                Deadline = request.Deadline,
                Status = SavingGoalStatus.Active
            };

            await _dbContext.SavingGoals.AddAsync(goal);
            await _dbContext.SaveChangesAsync();

            return (true, goal, null);
        }
        catch (Exception ex)
        {
            return (false, null, ex.Message);
        }
    }

    public async Task<SavingGoal?> GetGoalByIdAsync(Guid id, Guid userId) =>
        await _dbContext.SavingGoals
            .Include(g => g.Contributions)
            .FirstOrDefaultAsync(g => g.Id == id && g.UserId == userId);

    public async Task<IEnumerable<SavingGoal>> GetAllGoalsAsync(Guid userId, bool includeArchived = false) =>
        await _dbContext.SavingGoals
            .Where(g => g.UserId == userId && (includeArchived || !g.IsArchived))
            .OrderByDescending(g => g.CreatedAt)
            .ToListAsync();

    public async Task<(bool IsSuccess, string? ErrorMessage)> UpdateGoalAsync(Guid id, UpdateSavingGoalRequest request, Guid userId)
    {
        SavingGoal? goal = await _dbContext.SavingGoals.FirstOrDefaultAsync(g => g.Id == id && g.UserId == userId);

        if (goal is null) return (false, "Saving goal not found.");

        goal.Title = request.Title ?? goal.Title;
        goal.Description = request.Description ?? goal.Description;
        goal.TargetAmount = request.TargetAmount ?? goal.TargetAmount;
        goal.Deadline = request.Deadline ?? goal.Deadline;
        goal.Status = request.Status ?? goal.Status;
        goal.IsArchived = request.IsArchived ?? goal.IsArchived;
        goal.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();

        return (true, null);
    }

    public async Task<(bool IsSuccess, string? ErrorMessage)> DeleteGoalAsync(Guid id, Guid userId)
    {
        SavingGoal? goal = await _dbContext.SavingGoals.FirstOrDefaultAsync(g => g.Id == id && g.UserId == userId);

        if (goal is null) return (false, "Saving goal not found.");

        _dbContext.SavingGoals.Remove(goal);
        await _dbContext.SaveChangesAsync();

        return (true, null);
    }

    public async Task<(bool IsSuccess, string? Message)> ArchiveGoalAsync(Guid id, Guid userId, bool archiveGoal = true)
    {
        SavingGoal? goal = await _dbContext.SavingGoals.FirstOrDefaultAsync(g => g.Id == id && g.UserId == userId);

        if (goal is null) return (false, "Saving goal not found.");

        if (goal.IsArchived == archiveGoal)
        {
            string state = archiveGoal ? "already archived" : "already unarchived";
            return (true, $"Saving goal is {state}.");
        }

        goal.IsArchived = archiveGoal;
        goal.ArchivedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();

        string action = archiveGoal ? "archived." : "unarchived.";
        return (true, $"Saving goal {action}");
    }

    public async Task<(bool IsSuccess, string? ErrorMessage)> AddSavingContributionAsync(AddSavingContributionRequest request, Guid userId)
    {
        SavingGoal? goal = await _dbContext.SavingGoals.FirstOrDefaultAsync(g => g.Id == request.SavingGoalId && g.UserId == userId);

        if (goal is null) return (false, "Saving goal not found.");

        Expense expense = new()
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Amount = request.Amount,
            Category = ExpenseCategory.Savings,
            DateRecorded = DateTime.UtcNow,
            DateOfExpense = request.DateOfExpense ?? DateTime.UtcNow,
            PaymentMethod = request.PaymentMethod,
            Description = request.Description
        };

        SavingGoalContribution contribution = new()
        {
            Id = Guid.NewGuid(),
            SavingGoalId = goal.Id,
            ExpenseId = expense.Id,
            Amount = request.Amount,
            ContributedAt = DateTime.UtcNow
        };

        goal.CurrentAmount += request.Amount;

        await _dbContext.Expenses.AddAsync(expense);
        await _dbContext.SavingGoalContributions.AddAsync(contribution);
        await _dbContext.SaveChangesAsync();

        return (true, null);
    }
}
