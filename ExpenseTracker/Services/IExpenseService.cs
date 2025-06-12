using ExpenseTracker.Models;

namespace ExpenseTracker.Services;

public interface IExpenseService
{
    Task<(bool IsSuccess, Expense? Data, string? ErrorMessage)> CreateExpenseAsync(Expense expenseToCreate);
    Task<Expense?> GetExpenseByIdAsync(Guid id);
    Task<IEnumerable<Expense>> GetAllExpensesAsync();
    Task<bool> UpdateExpenseAsync(Guid id, Expense expenseToUpdate);
    Task<bool> DeleteExpenseAsync(Guid id);
    Task<bool> DeleteAllExpensesAsync();
}
