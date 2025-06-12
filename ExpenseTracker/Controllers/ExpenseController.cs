using ExpenseTracker.Models;
using ExpenseTracker.Services;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ExpenseController(IExpenseService expenseService) : ControllerBase
{
    private readonly IExpenseService _expenseService = expenseService;

    // POST: api/Expense
    [HttpPost]
    public async Task<IActionResult> CreateExpense([FromBody] Expense expense)
    {
        var result = await _expenseService.CreateExpenseAsync(expense);

    if (!result.IsSuccess)
    {
        return StatusCode(500, new ApiResponse<Expense>
        {
            Success = false,
            Message = result.ErrorMessage ?? "An error occurred",
            Data = null
        });
    }

    return CreatedAtAction(nameof(CreateExpense), new ApiResponse<Expense>
    {
        Success = true,
        Message = "Expense recorded",
        Data = result.Data
    });
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
    [HttpGet("getall")]
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
