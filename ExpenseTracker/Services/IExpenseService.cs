using ExpenseTracker.Enums;
using ExpenseTracker.Models;

public interface IExpenseService
{
    Task<(bool IsSuccess, Expense? Data, string? Message)> CreateExpenseAsync(Expense expenseToCreate, Guid userId);
    Task<Expense?> GetExpenseByIdAsync(Guid id, Guid userId);
    Task<IEnumerable<Expense>> GetFilteredExpensesAsync(
        Guid userId,
        DateTime? startDate = null,
        DateTime? endDate = null,
        decimal? minAmount = null,
        decimal? maxAmount = null,
        decimal? exactAmount = null,
        ExpenseCategory? category = null);
    Task<IEnumerable<Expense>> GetAllExpensesAsync(Guid userId);
    Task<double> GetTotalExpenseAsync(Guid userId, DateTime? startDate, DateTime? endDate, int? month, int? year);
    Task<bool> UpdateExpenseAsync(Guid id, Expense expenseToUpdate, Guid userId);
    Task<bool> DeleteExpenseAsync(Guid id, Guid userId);
    Task<bool> DeleteAllExpensesAsync(Guid userId);
}