public interface ISavingGoalService
{
    Task<(bool IsSuccess, SavingGoal? Data, string? ErrorMessage)> CreateGoalAsync(CreateSavingGoalRequest request, Guid userId);
    Task<(double target, double saved, double percentage)> GetSavingStatusAsync(Guid userId, int month, int year);
}
