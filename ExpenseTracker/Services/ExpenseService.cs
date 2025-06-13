using ExpenseTracker.Core.Enums;
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
            expenseToCreate.DateRecorded = DateTime.Now;
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

    public async Task<IEnumerable<Expense>> GetFilteredExpensesAsync(
        DateTime? startDate = null,
        DateTime? endDate = null,
        decimal? minAmount = null,
        decimal? maxAmount = null,
        decimal? exactAmount = null,
        int? category = null
    )
    {
        IQueryable<Expense> query = _dbContext.Expenses.AsQueryable();

        if (startDate.HasValue)
        {
            query = query.Where(e => e.DateRecorded >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(e => e.DateRecorded <= endDate.Value);
        }

        if (exactAmount.HasValue)
        {
            query = query.Where(e => (decimal)e.Amount == exactAmount.Value);
        }
        else
        {
            if (minAmount.HasValue)
            {
                query = query.Where(e => (decimal)e.Amount >= minAmount.Value);
            }

            if (maxAmount.HasValue)
            {
                query = query.Where(e => (decimal)e.Amount <= maxAmount.Value);
            }
        }

        if (category.HasValue)
        {
            query = query.Where(e => e.Category == (ExpenseCategory)category);
        }

        return await query.ToListAsync();
    }

    public async Task<bool> UpdateExpenseAsync(Guid id, Expense expenseToUpdate)
    {
        Expense? existingExpense = await _dbContext.Expenses.FindAsync(id);

        if (existingExpense is null) return false;

        existingExpense.Category = expenseToUpdate.Category;
        existingExpense.Amount = expenseToUpdate.Amount;
        existingExpense.DateOfExpense = expenseToUpdate.DateOfExpense;
        existingExpense.Description = expenseToUpdate.Description;

        _dbContext.Expenses.Update(existingExpense);
        await _dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteExpenseAsync(Guid id)
    {
        Expense? expenseToDelete = await _dbContext.Expenses.FindAsync(id);

        if (expenseToDelete is null) return false;

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
