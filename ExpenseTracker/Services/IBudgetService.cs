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
    /// Retrieves the total amount a user has spent in a specific category and month.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="category">The expense category.</param>
    /// <param name="month">The month (1-12) to filter by.</param>
    /// <param name="year">The year to filter by.</param>
    /// <returns>
    /// A <see cref="ServiceResult{T}"/> containing the total amount spent.
    /// </returns>
    Task<ServiceResult<double>> GetSpentAmountForCategoryAsync(Guid userId, ExpenseCategory category, int month, int year);

    /// <summary>
    /// Retrieves the user's budget and actual spending for a specific category and month.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="category">The category to evaluate.</param>
    /// <param name="month">The month (1-12).</param>
    /// <param name="year">The year.</param>
    /// <returns>
    /// A <see cref="ServiceResult{T}"/> containing budget and spending information.
    /// </returns>
    Task<ServiceResult<BudgetStatusResponse?>> GetBudgetStatusAsync(Guid userId, ExpenseCategory category, int month, int year);

    /// <summary>
    /// Retrieves a detailed summary of a user's budget for a given category and month,
    /// including budgeted amount, amount spent, percentage used, and an advisory message.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="category">The budget category.</param>
    /// <param name="month">The month (1-12).</param>
    /// <param name="year">The year.</param>
    /// <returns>
    /// A <see cref="BudgetSummaryResponse"/> object containing detailed budget performance information.
    /// </returns>
    Task<BudgetSummaryResponse> GetBudgetSummaryAsync(Guid userId, ExpenseCategory category, int month, int year);
}
