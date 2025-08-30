using ExpenseTracker.Enums;
using ExpenseTracker.Models;
using ExpenseTracker.Models.DTOs.Budget;
using ExpenseTracker.Models.DTOs.Budgets;

namespace ExpenseTracker.Services;

/// <summary>
/// Defines operations related to user budgeting functionality, including creation, updates, deletions, tracking, and summaries.
/// </summary>
public interface IBudgetService
{
    /// <summary>
    /// Creates a new budget for a specific user, category, and period.
    /// </summary>
    /// <param name="request">The request containing budget details such as category, limit, and period.</param>
    /// <param name="userId">The ID of the user creating the budget.</param>
    /// <returns>
    /// A <see cref="ServiceResult{T}"/> indicating success or failure, including the created budget details if successful.
    /// </returns>
    Task<ServiceResult<CreateBudgetResponse?>> CreateBudgetAsync(CreateBudgetRequest request, Guid userId);

    /// <summary>
    /// Calculates the total amount a user has spent in a specific expense category for a given month.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="category">The expense category to calculate spending for.</param>
    /// <param name="date">A date representing the target month and year.</param>
    /// <returns>
    /// A <see cref="double"/> representing the total amount spent by the user in the specified category and period.
    /// </returns>
    Task<double> GetSpentAmountForCategoryAsync(Guid userId, ExpenseCategory category, DateOnly date);

    /// <summary>
    /// Retrieves the user's budgeted and actual spending amounts for a specific category and period.
    /// </summary>
    /// <param name="userId">The ID of the user requesting the budget status.</param>
    /// <param name="request">
    /// The request containing the target <see cref="ExpenseCategory"/> and <see cref="DateOnly"/> period (month and year).
    /// </param>
    /// <returns>
    /// A <see cref="BudgetStatusResponse"/> containing both the budgeted and actual spending amounts for the specified category and period.
    /// </returns>
    Task<BudgetStatusResponse> GetBudgetStatusAsync(Guid userId, BudgetStatusRequest request);

    /// <summary>
    /// Retrieves an overview of all budget categories and their usage for a user during a specific period.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="period">The month and year for which to retrieve the overview.</param>
    /// <returns>
    /// A <see cref="BudgetOverviewSummaryResponse"/> summarizing usage across all budgeted categories.
    /// </returns>
    Task<BudgetOverviewSummaryResponse> GetBudgetOverviewAsync(Guid userId, DateOnly period);

    /// <summary>
    /// Updates an existing budget for the specified user using the provided values.
    /// </summary>
    /// <param name="userId">The ID of the user requesting the update.</param>
    /// <param name="budgetId">The ID of the budget the requesting update will be made to.</param>
    /// <param name="request">The update request containing the budget ID and optional new values.</param>
    /// <returns>A <see cref="ServiceResult{T}"/> indicating the result of the update operation.</returns>
    Task<ServiceResult<object?>> UpdateBudgetAsync(Guid userId, Guid budgetId, UpdateBudgetRequest request);

    /// <summary>
    /// Deletes a user's budget entry based on its unique identifier.
    /// </summary>
    /// <param name="userId">The ID of the user who owns the budget.</param>
    /// <param name="budgetId">The unique identifier of the budget to delete.</param>
    /// <returns>
    /// A <see cref="ServiceResult{T}"/> indicating success or failure of the deletion process.
    /// </returns>
    Task<ServiceResult<object?>> DeleteBudgetAsync(Guid userId, Guid budgetId);
}
