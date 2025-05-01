using ExpenseTracker.Models;

namespace ExpenseTracker.Services;

public interface IExpenseService
{
    Task<Expense> CreateExpenseAsync(Expense expenseToCreate);
    Task<Expense?> GetExpenseByIdAsync(Guid id);
    Task<IEnumerable<Expense>> GetAllExpensesAsync();
    Task<bool> UpdateExpenseAsync(Guid id, Expense expenseToUpdate);
    Task<bool> DeleteExpenseAsync(Guid id);
    Task<bool> DeleteAllExpensesAsync();
}
