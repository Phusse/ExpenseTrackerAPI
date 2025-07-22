using ExpenseTracker.Enums;
using ExpenseTracker.Models;
using ExpenseTracker.Models.DTOs.Budget;

namespace ExpenseTracker.Services;

/// <summary>
/// Defines operations related to user budgeting functionality, including creation, tracking, and summaries.
/// </summary>
public interface IBudgetService
{
    /// <summary>
    /// Creates a new budget for a specific user, category, and month.
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
    /// <param name="category">The expense category to filter by.</param>
    /// <param name="date">Any date within the target month and year.</param>
    /// <returns>
    /// A <see cref="double"/> representing the total amount spent by the user in the specified category and period.
    /// </returns>
    Task<double> GetSpentAmountForCategoryAsync(Guid userId, ExpenseCategory category, DateOnly date);

    /// <summary>
    /// Retrieves the user's budgeted and actual spending amounts for a specific category and month.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="category">The expense category to evaluate.</param>
    /// <param name="date">A date representing the target month and year.</param>
    /// <returns>
    /// A <see cref="BudgetStatusResponse"/> containing both the budgeted and spent amounts.
    /// </returns>
    Task<BudgetStatusResponse> GetBudgetStatusAsync(Guid userId, ExpenseCategory category, DateOnly date);

    /// <summary>
    /// Retrieves a detailed summary of a user's budget for a given category and month,
    /// including budgeted amount, amount spent, percentage used, and an advisory message.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="category">The budget category.</param>
    /// <param name="date">Any date within the target month and year.</param>
    /// <returns>
    /// A <see cref="BudgetSummaryResponse"/> object containing detailed budget performance information.
    /// </returns>
    Task<BudgetSummaryResponse> GetBudgetSummaryAsync(Guid userId, ExpenseCategory category, DateOnly date);
}
