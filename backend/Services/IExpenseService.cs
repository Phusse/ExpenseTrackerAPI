using ExpenseTracker.Models.DTOs.Expenses;

namespace ExpenseTracker.Services;

/// <summary>
/// Defines the contract for managing user expenses, including creation, retrieval, filtering, updates, and deletions.
/// </summary>
public interface IExpenseService
{
    /// <summary>
    /// Creates a new expense for the specified user.
    /// </summary>
    /// <param name="userId">The identifier of the user creating the expense.</param>
    /// <param name="request">The expense creation details including category, amount, date, and description.</param>
    /// <returns>A response containing the created expense data.</returns>
    Task<CreateExpenseResponse> CreateExpenseAsync(Guid userId, CreateExpenseRequest request);

    /// <summary>
    /// Retrieves a specific expense by its ID and the associated user.
    /// </summary>
    /// <param name="id">The unique identifier of the expense.</param>
    /// <param name="userId">The identifier of the user who owns the expense.</param>
    /// <returns>The expense response if found; otherwise, null.</returns>
    Task<CreateExpenseResponse?> GetExpenseByIdAsync(Guid id, Guid userId);

    /// <summary>
    /// Retrieves a filtered list of expenses based on date, amount, and category.
    /// </summary>
    /// <param name="userId">The identifier of the user whose expenses to query.</param>
    /// <param name="request">The filtering criteria including dates, amount ranges, and category.</param>
    /// <returns>A service result containing a list of matching expenses or an error message.</returns>
    Task<IEnumerable<CreateExpenseResponse>> GetFilteredExpensesAsync(Guid userId, FilteredExpenseRequest request);

    /// <summary>
    /// Retrieves all expenses for a given user.
    /// </summary>
    /// <param name="userId">The identifier of the user.</param>
    /// <returns>A service result containing the full list of the user's expenses.</returns>
    Task<IEnumerable<CreateExpenseResponse>> GetAllExpensesAsync(Guid userId);

    /// <summary>
    /// Calculates the total expense for a user over a specified time period.
    /// </summary>
    /// <param name="userId">The identifier of the user.</param>
    /// <param name="request">The time range or monthly/yearly filter details.</param>
    /// <returns>A service result containing the total expense amount.</returns>
    Task<double> GetTotalExpenseAsync(Guid userId, TotalExpenseRequest request);

    /// <summary>
    /// Updates an existing expense for a user.
    /// </summary>
    /// <param name="id">The ID of the expense to update.</param>
    /// <param name="request">The new expense details to apply.</param>
    /// <returns>True if the update was successful; otherwise, false.</returns>
    Task<bool> UpdateExpenseAsync(Guid id, UpdateExpenseRequest request);

    /// <summary>
    /// Deletes a specific expense for a user.
    /// </summary>
    /// <param name="id">The ID of the expense to delete.</param>
    /// <param name="userId">The identifier of the user who owns the expense.</param>
    /// <returns>True if deletion was successful; otherwise, false.</returns>
    Task<bool> DeleteExpenseAsync(Guid id, Guid userId);

    /// <summary>
    /// Deletes all expenses for a given user.
    /// </summary>
    /// <param name="userId">The identifier of the user.</param>
    /// <returns>True if all expenses were successfully deleted; otherwise, false.</returns>
    Task<bool> DeleteAllExpensesAsync(Guid userId);
}
