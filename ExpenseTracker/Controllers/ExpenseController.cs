using ExpenseTracker.Data;
using ExpenseTracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Controllers;

[ApiController, Route("api/[controller]")]
public class ExpenseController(ExpenseTrackerDbContext dbContext) : ControllerBase
{
    private readonly ExpenseTrackerDbContext _dbContext = dbContext;

    // POST: api/Expense
    [HttpPost]
    public async Task<ActionResult<Expense>> CreateExpense([FromBody] Expense newExpense)
    {
        if (newExpense is null)
        {
            return BadRequest("Expense data is required.");
        }

        newExpense.Id = Guid.NewGuid();
        await _dbContext.Expenses.AddAsync(newExpense);
        await _dbContext.SaveChangesAsync();

        return CreatedAtAction(nameof(GetExpenseById), new { id = newExpense.Id }, newExpense);
    }

    // GET: api/Expense/{id}
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Expense>> GetExpenseById(Guid id)
    {
        Expense? expense = await _dbContext.Expenses.FindAsync(id);

        if (expense is null)
        {
            return NotFound($"Expense with Id {id} not found.");
        }

        return Ok(expense);
    }

    // GET: api/Expense/all
    [HttpGet("all")]
    public async Task<ActionResult<IEnumerable<Expense>>> GetAllExpenses()
    {
        List<Expense> expenses = await _dbContext.Expenses.ToListAsync();
        return Ok(expenses);
    }

    // PUT: api/Expense/{id}
    [HttpPut("{id:guid}")]
    public async Task<ActionResult> UpdateExpense(Guid id, [FromBody] Expense updatedExpense)
    {
        if (updatedExpense == null || id != updatedExpense.Id)
        {
            return BadRequest("Expense data is invalid or Id mismatch.");
        }

        Expense? existingExpense = await _dbContext.Expenses.FindAsync(id);

        if (existingExpense is null)
        {
            return NotFound($"Expense with Id {id} not found.");
        }

        existingExpense.Category = updatedExpense.Category;
        existingExpense.Amount = updatedExpense.Amount;
        existingExpense.Date = updatedExpense.Date;
        existingExpense.Description = updatedExpense.Description;

        _dbContext.Expenses.Update(existingExpense);
        await _dbContext.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/Expense/{id}
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteExpense(Guid id)
    {
        Expense? expenseToDelete = await _dbContext.Expenses.FindAsync(id);

        if (expenseToDelete is null)
        {
            return NotFound($"Expense with Id {id} not found.");
        }

        _dbContext.Expenses.Remove(expenseToDelete);
        await _dbContext.SaveChangesAsync();

        return NoContent();
    }
}
