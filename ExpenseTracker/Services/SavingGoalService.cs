using ExpenseTracker.Data;
using ExpenseTracker.Enums;
using Microsoft.EntityFrameworkCore;

public class SavingGoalService : ISavingGoalService
{
    private readonly ExpenseTrackerDbContext _dbContext;

    public SavingGoalService(ExpenseTrackerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<(bool IsSuccess, SavingGoal? Data, string? ErrorMessage)> CreateGoalAsync(CreateSavingGoalRequest request, Guid userId)
    {
        try
        {
            var exists = await _dbContext.SavingGoals
                .FirstOrDefaultAsync(g => g.UserId == userId && g.Month == request.Month && g.Year == request.Year);

            if (exists != null)
                return (false, null, "You already set a saving goal for this period.");

            var goal = new SavingGoal
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                TargetAmount = request.TargetAmount,
                Month = request.Month,
                Year = request.Year
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

    public async Task<(double target, double saved, double percentage)> GetSavingStatusAsync(Guid userId, int month, int year)
    {
        var goal = await _dbContext.SavingGoals
            .FirstOrDefaultAsync(g => g.UserId == userId && g.Month == month && g.Year == year);

        var saved = await _dbContext.Expenses
            .Where(e => e.UserId == userId &&
                        e.Category == ExpenseCategory.Savings &&
                        e.DateOfExpense != null &&
                        e.DateOfExpense.Value.Month == month &&
                        e.DateOfExpense.Value.Year == year)
            .SumAsync(e => e.Amount);

        var target = goal?.TargetAmount ?? 0;
        var percentage = target == 0 ? 0 : (saved / target) * 100;

        return (target, saved, Math.Round(percentage, 2));
    }
}
