using ExpenseTracker.Models;
using ExpenseTracker.Models.DTOs;

namespace ExpenseTracker.Services;

/// <summary>
/// Service interface for managing user saving goals.
/// </summary>
public interface ISavingGoalService
{
    /// <summary>
    /// Creates a new saving goal for the specified user.
    /// </summary>
    /// <param name="request">The saving goal creation request.</param>
    /// <param name="userId">The user's unique identifier.</param>
    /// <returns>
    /// Tuple: (IsSuccess, Created SavingGoal or null, ErrorMessage or null)
    /// </returns>
    Task<(bool IsSuccess, SavingGoal? Data, string? ErrorMessage)> CreateGoalAsync(CreateSavingGoalRequest request, Guid userId);

    /// <summary>
    /// Retrieves a saving goal by its identifier for the specified user.
    /// </summary>
    /// <param name="id">The saving goal's unique identifier.</param>
    /// <param name="userId">The user's unique identifier.</param>
    /// <returns>The found SavingGoal or null.</returns>
    Task<SavingGoal?> GetGoalByIdAsync(Guid id, Guid userId);

    /// <summary>
    /// Retrieves all saving goals for the specified user.
    /// </summary>
    /// <param name="userId">The user's unique identifier.</param>
    /// <param name="includeArchived">Whether to include archived goals.</param>
    /// <returns>Enumerable of SavingGoal.</returns>
    Task<IEnumerable<SavingGoal>> GetAllGoalsAsync(Guid userId, bool includeArchived = false);

    /// <summary>
    /// Updates an existing saving goal for the specified user.
    /// </summary>
    /// <param name="id">The saving goal's unique identifier.</param>
    /// <param name="request">The update request.</param>
    /// <param name="userId">The user's unique identifier.</param>
    /// <returns>
    /// Tuple: (IsSuccess, ErrorMessage or null)
    /// </returns>
    Task<(bool IsSuccess, string? ErrorMessage)> UpdateGoalAsync(Guid id, UpdateSavingGoalRequest request, Guid userId);

    /// <summary>
    /// Adds a contribution to a user's saving goal.
    /// </summary>
    /// <param name="request">The request containing contribution details and the target saving goal.</param>
    /// <param name="userId">The user's unique identifier.</param>
    /// <returns>
    /// Tuple: (IsSuccess, ErrorMessage or null)
    /// </returns>
    Task<(bool IsSuccess, string? ErrorMessage)> AddSavingContributionAsync(AddSavingContributionRequest request, Guid userId);

    /// <summary>
    /// Deletes a saving goal for the specified user.
    /// </summary>
    /// <param name="id">The saving goal's unique identifier.</param>
    /// <param name="userId">The user's unique identifier.</param>
    /// <returns>
    /// Tuple: (IsSuccess, ErrorMessage or null)
    /// </returns>
    Task<(bool IsSuccess, string? ErrorMessage)> DeleteGoalAsync(Guid id, Guid userId);

    /// <summary>
    /// Archives a saving goal for the specified user.
    /// </summary>
    /// <param name="id">The saving goal's unique identifier.</param>
    /// <param name="userId">The user's unique identifier.</param>
    /// <param name="archiveGoal">The archive flag, defaults to true if not provided.</param>
    /// <returns>
    /// Tuple: (IsSuccess, Message or null)
    /// </returns>
    Task<(bool IsSuccess, string? Message)> ArchiveGoalAsync(Guid id, Guid userId, bool archiveGoal = true);
}
