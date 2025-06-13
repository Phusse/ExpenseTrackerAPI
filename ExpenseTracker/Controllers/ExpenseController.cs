using System;
using ExpenseTracker.Models;
using ExpenseTracker.Models.Responses;
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
        Console.WriteLine("Retuning created expense");
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

    [HttpGet("filter")]
    public async Task<IActionResult> GetFilteredExpenses(
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate,
        [FromQuery] decimal? minAmount,
        [FromQuery] decimal? maxAmount,
        [FromQuery] decimal? exactAmount,
        [FromQuery] string? category)
    {
        try
        {
           var expenses = await _expenseService.GetFilteredExpensesAsync(startDate, endDate, minAmount, maxAmount, exactAmount, category);

            if (!expenses.Any())
            {
                return NotFound(new
                {
                    success = false,
                    message = "No matching expenses found."
                });
            }

            return Ok(new
            {
                success = true,
                message = "Filtered expenses retrieved successfully.",
                data = expenses
            });
        }
        catch (Exception ex)
        {
            // Optional: Log the error
            Console.WriteLine($"Error filtering expenses: {ex.Message}");

            return StatusCode(500, new
            {
                success = false,
                message = "An error occurred while retrieving expenses.",
                error = ex.Message
            });
        }
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
