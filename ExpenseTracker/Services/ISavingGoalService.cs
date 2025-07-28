using ExpenseTracker.Models;
using ExpenseTracker.Models.DTOs.SavingGoals;

namespace ExpenseTracker.Services;

/// <summary>
/// Defines the contract for managing user saving goals.
/// </summary>
public interface ISavingGoalService
{
    /// <summary>
    /// Creates a new saving goal for a user.
    /// </summary>
    /// <param name="request">The details of the saving goal to create.</param>
    /// <param name="userId">The ID of the user creating the goal.</param>
    /// <returns>A result containing the created goal and status information or null.</returns>
    Task<ServiceResult<CreateSavingGoalResponse?>> CreateGoalAsync(CreateSavingGoalRequest request, Guid userId);

    /// <summary>
    /// Retrieves a specific saving goal for a user by its ID.
    /// </summary>
    /// <param name="id">The ID of the saving goal.</param>
    /// <param name="userId">The ID of the user who owns the goal.</param>
    /// <returns>The saving goal if found; otherwise, <c>null</c>.</returns>
    Task<CreateSavingGoalResponse?> GetGoalByIdAsync(Guid id, Guid userId);

    /// <summary>
    /// Retrieves all saving goals for a user.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="includeArchived">Indicates whether to include archived goals.</param>
    /// <returns>A list of saving goals.</returns>
    Task<IEnumerable<CreateSavingGoalResponse>> GetAllGoalsAsync(Guid userId, bool includeArchived = false);

    /// <summary>
    /// Updates an existing saving goal for a user.
    /// </summary>
    /// <param name="id">The ID of the saving goal to update.</param>
    /// <param name="request">The updated saving goal data.</param>
    /// <param name="userId">The ID of the user.</param>
    /// <returns>A result indicating success or failure of the update.</returns>
    Task<ServiceResult<object?>> UpdateGoalAsync(Guid id, UpdateSavingGoalRequest request, Guid userId);

    /// <summary>
    /// Adds a contribution to an existing saving goal.
    /// </summary>
    /// <param name="request">The contribution details and target goal ID.</param>
    /// <param name="userId">The ID of the user making the contribution.</param>
    /// <returns>A result indicating success or failure of the contribution.</returns>
    Task<ServiceResult<object?>> AddSavingContributionAsync(AddSavingContributionRequest request, Guid userId);

    /// <summary>
    /// Deletes a saving goal for a user.
    /// </summary>
    /// <param name="id">The ID of the saving goal to delete.</param>
    /// <param name="userId">The ID of the user.</param>
    /// <returns>A result indicating success or failure of the deletion.</returns>
    Task<ServiceResult<object?>> DeleteGoalAsync(Guid id, Guid userId);

    /// <summary>
    /// Archives or unarchives a saving goal.
    /// </summary>
    /// <param name="id">The ID of the saving goal.</param>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="archiveGoal">If <c>true</c>, the goal will be archived; otherwise, it will be unarchived. Default is <c>true</c>.</param>
    /// <returns>A result indicating success or failure of the operation.</returns>
    Task<ServiceResult<object?>> ArchiveGoalAsync(Guid id, Guid userId, bool archiveGoal = true);
}
