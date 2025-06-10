using ExpenseTracker.Models;
using ExpenseTracker.Services;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Controllers;

[ApiController, Route("api/[controller]")]
public class ExpenseController(IExpenseService expenseService) : ControllerBase
{
    private readonly IExpenseService _expenseService = expenseService;

    // POST: api/Expense
    [HttpPost]
    public async Task<ActionResult<Expense>> CreateExpenseAsync([FromBody] Expense expenseToCreate)
    {
        if (expenseToCreate is null)
        {
            return BadRequest("Expense data is required.");
        }

        Expense createdExpense = await _expenseService.CreateExpenseAsync(expenseToCreate);
        return Ok(createdExpense);
    }

    // GET: api/Expense/{id}
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Expense>> GetExpenseByIdAsync(Guid id)
    {
        Expense? expense = await _expenseService.GetExpenseByIdAsync(id);

        if (expense is null)
        {
            return NotFound($"Expense with Id {id} not found.");
        }

        return Ok(expense);
    }

    // GET: api/Expense/all
    [HttpGet("all")]
    public async Task<ActionResult<IEnumerable<Expense>>> GetAllExpensesAsync()
    {
        IEnumerable<Expense> expenses = await _expenseService.GetAllExpensesAsync();
        return Ok(expenses);
    }

    // PUT: api/Expense/{id}
    [HttpPut("{id:guid}")]
    public async Task<ActionResult> UpdateExpenseAsync(Guid id, [FromBody] Expense expenseToUpdate)
    {
        if (expenseToUpdate is null || id != expenseToUpdate.Id)
        {
            return BadRequest("Expense data is invalid or Id mismatch.");
        }

        bool updateSuccessful = await _expenseService.UpdateExpenseAsync(id, expenseToUpdate);

        if (!updateSuccessful)
        {
            return NotFound($"Expense with Id {id} not found.");
        }

        return NoContent();
    }

    // DELETE: api/Expense/{id}
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteExpenseAsync(Guid id)
    {
        bool deleteSuccessful = await _expenseService.DeleteExpenseAsync(id);

        if (!deleteSuccessful)
        {
            return NotFound($"Expense with Id {id} not found.");
        }

        return NoContent();
    }

    // DELETE: api/Expense/all
    [HttpDelete("all")]
    public async Task<ActionResult> DeleteAllExpensesAsync()
    {
        bool deleteSuccessful = await _expenseService.DeleteAllExpensesAsync();

        if (!deleteSuccessful)
        {
            return NotFound($"Failed to delete all expenses.");
        }

        return NoContent();
    }
}
