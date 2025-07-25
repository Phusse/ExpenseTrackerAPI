using ExpenseTracker.Data;
using ExpenseTracker.Enums;
using ExpenseTracker.Models;
using ExpenseTracker.Models.DTOs.Expenses;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Services;

/// <summary>
/// Provides services for managing user expenses, including creating, retrieving, filtering, updating, and deleting expense records,
/// as well as calculating total expenses within specified criteria.
/// </summary>
internal class ExpenseService(ExpenseTrackerDbContext dbContext) : IExpenseService
{
    private readonly ExpenseTrackerDbContext _dbContext = dbContext;

    public async Task<CreateExpenseResponse> CreateExpenseAsync(Guid userId, CreateExpenseRequest request)
    {
        Expense expense = new()
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Category = request.Category,
            Amount = request.Amount,
            DateRecorded = DateTime.UtcNow,
            DateOfExpense = request.DateOfExpense,
            PaymentMethod = request.PaymentMethod,
            Description = request.Description,
        };

        await _dbContext.Expenses.AddAsync(expense);
        await _dbContext.SaveChangesAsync();

        return new CreateExpenseResponse
        {
            Id = expense.Id,
            Category = expense.Category,
            Amount = expense.Amount,
            DateOfExpense = expense.DateOfExpense,
            DateRecorded = expense.DateRecorded,
            PaymentMethod = expense.PaymentMethod,
            Description = expense.Description,
        };
    }

    public async Task<CreateExpenseResponse?> GetExpenseByIdAsync(Guid id, Guid userId)
    {
        Expense? expense = await _dbContext.Expenses
            .FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId);

        if (expense is null) return null;

        return new CreateExpenseResponse
        {
            Id = expense.Id,
            Category = expense.Category,
            Amount = expense.Amount,
            DateOfExpense = expense.DateOfExpense,
            DateRecorded = expense.DateRecorded,
            PaymentMethod = expense.PaymentMethod,
            Description = expense.Description,
        };
    }

    public async Task<IEnumerable<CreateExpenseResponse>> GetAllExpensesAsync(Guid userId)
    {
        List<CreateExpenseResponse> result = await _dbContext.Expenses
            .Where(e => e.UserId == userId)
            .OrderByDescending(e => e.DateRecorded)
            .Select(e => new CreateExpenseResponse
            {
                Id = e.Id,
                Category = e.Category,
                Amount = e.Amount,
                DateOfExpense = e.DateOfExpense,
                DateRecorded = e.DateRecorded,
                PaymentMethod = e.PaymentMethod,
                Description = e.Description,
            })
            .ToListAsync();

        return result;
    }

    public async Task<IEnumerable<CreateExpenseResponse>> GetFilteredExpensesAsync(Guid userId, FilteredExpenseRequest request)
    {
        IQueryable<Expense> query = _dbContext.Expenses.Where(e => e.UserId == userId);

        if (request.StartDate.HasValue)
            query = query.Where(e => e.DateOfExpense >= request.StartDate.Value);

        if (request.EndDate.HasValue)
            query = query.Where(e => e.DateOfExpense <= request.EndDate.Value);

        if (request.ExactAmount.HasValue)
            query = query.Where(e => (decimal)e.Amount == request.ExactAmount.Value);
        else
        {
            if (request.MinAmount.HasValue)
                query = query.Where(e => (decimal)e.Amount >= request.MinAmount.Value);

            if (request.MaxAmount.HasValue)
                query = query.Where(e => (decimal)e.Amount <= request.MaxAmount.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Category) && Enum.TryParse<ExpenseCategory>(request.Category, out var parsedCategory))
        {
            query = query.Where(e => e.Category == parsedCategory);
        }

        List<CreateExpenseResponse> filteredResult = await query.OrderByDescending(e => e.DateRecorded)
            .Select(e => new CreateExpenseResponse
            {
                Id = e.Id,
                Category = e.Category,
                Amount = e.Amount,
                DateOfExpense = e.DateOfExpense,
                DateRecorded = e.DateRecorded,
                PaymentMethod = e.PaymentMethod,
                Description = e.Description,
            })
            .ToListAsync();

        return filteredResult;
    }

    public async Task<double> GetTotalExpenseAsync(Guid userId, TotalExpenseRequest request)
    {
        IQueryable<Expense> query = _dbContext.Expenses.Where(e => e.UserId == userId);

        if (request.Month.HasValue && request.Year.HasValue)
        {
            query = query.Where(e => e.DateOfExpense.Month == request.Month.Value && e.DateOfExpense.Year == request.Year.Value);
        }
        else if (request.StartDate.HasValue && request.EndDate.HasValue)
        {
            query = query.Where(e => e.DateOfExpense >= request.StartDate.Value && e.DateOfExpense <= request.EndDate.Value);
        }
        else if (request.Year.HasValue)
        {
            query = query.Where(e => e.DateOfExpense.Year == request.Year.Value);
        }
        else if (request.Month.HasValue)
        {
            var currentYear = DateTime.UtcNow.Year;
            query = query.Where(e => e.DateOfExpense.Month == request.Month.Value && e.DateOfExpense.Year == currentYear);
        }

        return await query.SumAsync(e => e.Amount);
    }

    public async Task<bool> UpdateExpenseAsync(Guid userId, UpdateExpenseRequest request)
    {
        Expense? existing = await _dbContext.Expenses
            .FirstOrDefaultAsync(e => e.Id == request.Id && e.UserId == userId);

        if (existing is null) return false;

        if (request.Category.HasValue)
            existing.Category = request.Category.Value;

        if (request.Amount.HasValue)
            existing.Amount = request.Amount.Value;

        if (request.DateOfExpense.HasValue)
            existing.DateOfExpense = request.DateOfExpense.Value;

        if (request.PaymentMethod.HasValue)
            existing.PaymentMethod = request.PaymentMethod.Value;

        if (request.Description is not null)
            existing.Description = request.Description;

        _dbContext.Expenses.Update(existing);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteExpenseAsync(Guid id, Guid userId)
    {
        Expense? expense = await _dbContext.Expenses.FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId);

        if (expense is null) return false;

        _dbContext.Expenses.Remove(expense);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAllExpensesAsync(Guid userId)
    {
        List<Expense> expenses = await _dbContext.Expenses.Where(e => e.UserId == userId).ToListAsync();

        if (expenses.Count == 0) return false;

        _dbContext.Expenses.RemoveRange(expenses);
        await _dbContext.SaveChangesAsync();
        return true;
    }
}
