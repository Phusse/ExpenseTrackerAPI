using System;
using ExpenseTracker.Models;
using ExpenseTracker.Services;
using Microsoft.AspNetCore.Mvc;
using ExpenseTracker.Contracts;
using System.Globalization;

namespace ExpenseTracker.Controllers;

[ApiController]
public class ExpenseController(IExpenseService expenseService) : ControllerBase
{
    private readonly IExpenseService _expenseService = expenseService;

    // POST: api/Expense
    [HttpPost]
    [Route(ExpenseRoutes.PostUrl.Create)]
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

    [HttpGet]
    [Route(ExpenseRoutes.GetUrl.GetById)]
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
    [HttpGet]
    [Route(ExpenseRoutes.GetUrl.GetAll)]
    public async Task<ActionResult<IEnumerable<Expense>>> GetAllExpensesAsync()
    {
        IEnumerable<Expense> expenses = await _expenseService.GetAllExpensesAsync();
        return Ok(expenses);
    }

    [HttpGet]
    [Route(ExpenseRoutes.GetUrl.Filter)]
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

    [HttpGet(ExpenseRoutes.GetUrl.Total)]
    public async Task<IActionResult> GetTotalExpense(
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate,
        [FromQuery] int? month,
        [FromQuery] int? year)
    {
        try
        {
            // Validate month
            if (month.HasValue && (month < 1 || month > 12))
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Month must be between 1 and 12."
                });
            }

            // Validate year
            if (year.HasValue && year < 1)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Year must be a valid positive number."
                });
            }

            var total = await _expenseService.GetTotalExpenseAsync(startDate, endDate, month, year);

            if (total == 0)
            {
                return Ok(new
                {
                    success = false,
                    message = "No matching expenses found.",
                    totalExpense = 0
                });
            }

            // Dynamic message
            string message;
            if (month.HasValue && year.HasValue)
            {
                var monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month.Value);
                message = $"Total expense for {monthName} {year} is: {total}";
            }
            else if (startDate.HasValue || endDate.HasValue)
            {
                message = $"Total expense from {startDate?.ToString("yyyy-MM-dd") ?? "beginning"} to {endDate?.ToString("yyyy-MM-dd") ?? "now"} is: {total}";
            }
            else if (year.HasValue)
            {
                message = $"Total expense for the year {year} is: {total}";
            }
            else if (month.HasValue)
            {
                var monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month.Value);
                message = $"Total expense for {monthName} is: ${total}";
            }
            else
            {
                message = $"Total expense is: {total}";
            }

            return Ok(new
            {
                success = true,
                message
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                success = false,
                message = "An error occurred while calculating total expense.",
                error = ex.Message
            });
        }
    }


    // PUT: api/Expense/{id}
    [HttpPut]
    [Route(ExpenseRoutes.PutUrl.Update)]
    public async Task<ActionResult> UpdateExpenseAsync(Guid id, [FromBody] Expense expenseToUpdate)
    {
        if (expenseToUpdate == null || id != expenseToUpdate.Id)
        {
            return BadRequest(new { success = false, message = "Invalid data or ID mismatch." });
        }

        var success = await _expenseService.UpdateExpenseAsync(id, expenseToUpdate);

        if (!success)
        {
            return NotFound(new { success = false, message = $"Expense with ID {id} not found." });
        }

        return Ok(new { success = true, message = "Expense updated successfully." });
    }

    // DELETE: api/Expense/{id}
    [HttpDelete]
    [Route(ExpenseRoutes.DeleteUrl.Delete)]
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
    [HttpDelete]
    [Route(ExpenseRoutes.DeleteUrl.DeleteAll)]
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
