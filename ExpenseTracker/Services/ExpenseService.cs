using ExpenseTracker.Data;
using ExpenseTracker.Enums;
using ExpenseTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Services;

public class ExpenseService : IExpenseService
{
    private readonly ExpenseTrackerDbContext _dbContext;
    private readonly IEmailService _emailService;

    public ExpenseService(ExpenseTrackerDbContext dbContext, IEmailService emailService)
    {
        _dbContext = dbContext;
        _emailService = emailService;
    }

public async Task<(bool IsSuccess, Expense? Data, string? ErrorMessage)> CreateExpenseAsync(Expense expenseToCreate, Guid userId)
{
    try
    {
        expenseToCreate.Id = Guid.NewGuid();
        expenseToCreate.DateRecorded = DateTime.UtcNow;
        expenseToCreate.UserId = userId;

        await _dbContext.Expenses.AddAsync(expenseToCreate);
        await _dbContext.SaveChangesAsync();

        // Fetch user to get email and name
        var user = await _dbContext.Users.FindAsync(userId);
        if (user != null)
        {
            string subject = "New Expense Recorded";
            string message = $"Hi {user.Name},<br><br>You just added an expense of <strong>₦{expenseToCreate.Amount}</strong> in category <strong>{expenseToCreate.Category}</strong>.<br><br>Thanks for tracking your spending with us!";
            await _emailService.SendEmailAsync(user.Email, subject, message);
        }

        return (true, expenseToCreate, null);
    }
    catch (Exception ex)
    {
        return (false, null, ex.Message);
    }
}


    public async Task<Expense?> GetExpenseByIdAsync(Guid id, Guid userId)
    {
        return await _dbContext.Expenses
            .FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId);
    }

    public async Task<IEnumerable<Expense>> GetAllExpensesAsync(Guid userId)
    {
        return await _dbContext.Expenses
            .Where(e => e.UserId == userId)
            .OrderByDescending(e => e.DateRecorded)
            .ToListAsync();
    }

    public async Task<IEnumerable<Expense>> GetFilteredExpensesAsync(
        Guid userId,
        DateTime? startDate = null,
        DateTime? endDate = null,
        decimal? minAmount = null,
        decimal? maxAmount = null,
        decimal? exactAmount = null,
        string? category = null)
    {
        IQueryable<Expense> query = _dbContext.Expenses
            .Where(e => e.UserId == userId);

        if (startDate.HasValue)
        {
            query = query.Where(e => e.DateOfExpense >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(e => e.DateOfExpense <= endDate.Value);
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

        if (!string.IsNullOrWhiteSpace(category))
        {
            if (Enum.TryParse<ExpenseCategory>(category, out var parsedCategory))
            {
                query = query.Where(e => e.Category == parsedCategory);
            }
            else
            {
                // If parsing fails, return empty result
                return new List<Expense>();
            }
        }

        return await query.OrderByDescending(e => e.DateRecorded).ToListAsync();
    }

    public async Task<double> GetTotalExpenseAsync(Guid userId, DateTime? startDate, DateTime? endDate, int? month, int? year)
    {
        IQueryable<Expense> query = _dbContext.Expenses
            .Where(e => e.UserId == userId);

        // Filter by month and year
        if (month.HasValue && year.HasValue)
        {
            query = query.Where(e => e.DateOfExpense.HasValue &&
                                 e.DateOfExpense.Value.Month == month.Value &&
                                 e.DateOfExpense.Value.Year == year.Value);
        }
        // Filter by date range
        else if (startDate.HasValue && endDate.HasValue)
        {
            query = query.Where(e => e.DateOfExpense.HasValue &&
                                e.DateOfExpense.Value >= startDate.Value &&
                                e.DateOfExpense.Value <= endDate.Value);
        }
        // Filter only by year
        else if (year.HasValue)
        {
            query = query.Where(e => e.DateOfExpense.HasValue &&
                                     e.DateOfExpense.Value.Year == year.Value);
        }
        // Filter only by month (with current year fallback)
        else if (month.HasValue)
        {
            var currentYear = DateTime.UtcNow.Year;
            query = query.Where(e => e.DateOfExpense.HasValue &&
                                     e.DateOfExpense.Value.Month == month.Value &&
                                     e.DateOfExpense.Value.Year == currentYear);
        }

        double total = await query.SumAsync(e => e.Amount);
        return total;
    }

    public async Task<bool> UpdateExpenseAsync(Guid id, Expense expenseToUpdate, Guid userId)
    {
        Expense? existingExpense = await _dbContext.Expenses
            .FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId);

        if (existingExpense == null) return false;

        existingExpense.Category = expenseToUpdate.Category;
        existingExpense.Amount = expenseToUpdate.Amount;
        existingExpense.DateOfExpense = expenseToUpdate.DateOfExpense;
        existingExpense.Description = expenseToUpdate.Description;
        existingExpense.PaymentMethod = expenseToUpdate.PaymentMethod;

        _dbContext.Expenses.Update(existingExpense);
        await _dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteExpenseAsync(Guid id, Guid userId)
    {
        Expense? expenseToDelete = await _dbContext.Expenses
            .FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId);

        if (expenseToDelete == null) return false;

        _dbContext.Expenses.Remove(expenseToDelete);
        await _dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteAllExpensesAsync(Guid userId)
    {
        List<Expense> expenses = await _dbContext.Expenses
            .Where(e => e.UserId == userId)
            .ToListAsync();

        if (expenses.Count == 0) return false;

        _dbContext.Expenses.RemoveRange(expenses);
        await _dbContext.SaveChangesAsync();
        return true;
    }
}