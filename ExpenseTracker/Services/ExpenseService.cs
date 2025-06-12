using ExpenseTracker.Data;
using ExpenseTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Services;

public class ExpenseService(ExpenseTrackerDbContext dbContext) : IExpenseService
{
    private readonly ExpenseTrackerDbContext _dbContext = dbContext;

    public async Task<(bool IsSuccess, Expense? Data, string? ErrorMessage)> CreateExpenseAsync(Expense expenseToCreate)
    {
        try
        {
            expenseToCreate.Id = Guid.NewGuid();
            await _dbContext.Expenses.AddAsync(expenseToCreate);
            await _dbContext.SaveChangesAsync();

            return (true, expenseToCreate, null);
        }
        catch (Exception ex)
        {
            return (false, null, ex.Message);
        }
    }

    public async Task<Expense?> GetExpenseByIdAsync(Guid id)
    {
        Expense? expense = await _dbContext.Expenses.FindAsync(id);
        return expense;
    }

    public async Task<IEnumerable<Expense>> GetAllExpensesAsync()
    {
        List<Expense> expenses = await _dbContext.Expenses.ToListAsync();
        return expenses;
    }

    public async Task<bool> UpdateExpenseAsync(Guid id, Expense expenseToUpdate)
    {
        Expense? existingExpense = await _dbContext.Expenses.FindAsync(id);

        if (existingExpense == null) return false;

        existingExpense.Category = expenseToUpdate.Category;
        existingExpense.Amount = expenseToUpdate.Amount;
        existingExpense.Date = expenseToUpdate.Date;
        existingExpense.Description = expenseToUpdate.Description;

        _dbContext.Expenses.Update(existingExpense);
        await _dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteExpenseAsync(Guid id)
    {
        Expense? expenseToDelete = await _dbContext.Expenses.FindAsync(id);

        if (expenseToDelete == null) return false;

        _dbContext.Expenses.Remove(expenseToDelete);
        await _dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteAllExpensesAsync()
    {
        List<Expense> expenses = await _dbContext.Expenses.ToListAsync();

        if (expenses.Count == 0) return false;

        foreach (var expense in expenses)
        {
            _dbContext.Expenses.Remove(expense);
        }

        await _dbContext.SaveChangesAsync();
        return true;
    }
}
